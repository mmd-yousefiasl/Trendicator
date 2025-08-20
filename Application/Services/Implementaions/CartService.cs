using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Cart.ClientSide;
using Application.ViewModels.CartItem.ClientSide;
using Data.Context;
using Domain.Enums.Shopping;
using Domain.Models.Shopping;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls.Crypto.Impl;

namespace Application.Services.Implementaions
{
    public class CartService : ICartService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Cart> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly ICartItemService _itemService;
        private readonly IPaymentDataService _paymentDataService;
        private readonly ICouponService _couponService;
        #endregion

        #region Ctor
        public CartService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IHttpContextAccessor httpContextAccessor,
            IProductService productService,
            ICartItemService itemService,
            IPaymentDataService paymentDataService,
            ICouponService couponService)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _itemService = itemService;
            _paymentDataService = paymentDataService;
            _couponService = couponService;
        }
        #endregion

        public async Task<Result> AddToCart(int optionId)
        {
            if (optionId < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var productIdResult = await _productService.ClientSideGetProductIdByOptionId(optionId);
            if (productIdResult.IsFailure) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => (n.UserId == userId || n.UserIP == userIP) && n.Status == CartStatusEnum.PendingPayment))
            {
                var cartId = await _dbSet.Where(n => (n.UserId == userId || n.UserIP == userIP) &&
                n.Status == CartStatusEnum.PendingPayment).Select(n => n.Id).FirstOrDefaultAsync();
                if (cartId < 1) return Result.Failure(Messages.OperationFailedError);

                var itemResult = await _itemService.ClientSideCreateNewCartItem(new ClientSideCreateNewCartItemViewModel()
                {
                    CartId = cartId,
                    ProductId = productIdResult.Value,
                    OptionId = optionId
                });
                if (itemResult.IsFailure) return Result.Failure(itemResult.Message);
            }
            else
            {
                Cart cart = new Cart()
                {
                    IsPaid = false,
                    UserIP = userIP,
                    UserId = userId,
                    Status = CartStatusEnum.PendingPayment,
                    Items = new List<CartItem>()
                };

                cart.Items.Add(new CartItem()
                {
                    CartId = cart.Id,
                    ProductId = productIdResult.Value,
                    StockOptionId = optionId,
                });

                await _dbSet.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return Result.Success(Messages.AddedToCartSuccessfully);
        }

        public async Task<Result<List<ClientSideCartViewModel>>> ClientSideGetCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var items = await _dbSet
                .Where(cart => (cart.UserId == userId || cart.UserIP == userIP)
                            && cart.Status == CartStatusEnum.PendingPayment)
                .SelectMany(cart => cart.Items)
                .Select(item => new
                {
                    item.Id,
                    item.Product.IsAvailable,
                    item.Product.BannerFileName,
                    item.Product.Title,
                    item.Product.Slug,
                    Option = item.Product.Options.FirstOrDefault(o => o.Id == item.StockOptionId)
                })
                .Select(x => new ClientSideCartViewModel
                {
                    Id = x.Id,
                    FileName = x.BannerFileName,
                    IsAvailable = x.IsAvailable,
                    Title = x.Title,
                    Slug = x.Slug,
                    Price = x.Option != null ? x.Option.Price : 0,
                    OrderPrice = x.Option != null ? x.Option.OrderPrice : 0,
                    OptionTitle = x.Option != null ? x.Option.Title : ""
                })
                .AsNoTracking()
                .ToListAsync();
            if (!items.Any()) return Result.Failure<List<ClientSideCartViewModel>>(Messages.OperationFailedError);

            return items;
        }

        public async Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary()
        {
            var cartItemsResult = await ClientSideGetCart();
            if (cartItemsResult.IsFailure) return Result.Failure<ClientSideCartSummaryViewModel>(cartItemsResult.Message);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var cartItems = cartItemsResult.Value;

            ClientSideCartSummaryViewModel summary = new ClientSideCartSummaryViewModel();
            summary.SubTotal = cartItems.Where(n => n.IsAvailable).Sum(n => n.Price);
            summary.TotalDiscount = cartItems.Where(n => n.OrderPrice > 0 && n.IsAvailable).Sum(n => n.Price - n.OrderPrice);
            summary.SubTotalAfterDisocunt = summary.SubTotal - (summary.TotalDiscount ?? 0);

            var cartCode = await _dbSet.Where(n => (n.UserId == userId || n.UserIP == userIP) &&
            n.Status == CartStatusEnum.PendingPayment)
                .Select(n => n.DiscountCode)
                .FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(cartCode))
            {
                var percentResult = await _couponService.ClientSideGetCouponPercentByCode(cartCode);
                if (percentResult.IsSuccess)
                {
                    summary.DiscountCode = cartCode;
                    summary.DiscountPercent = percentResult.Value;
                    summary.DiscountCodeAmount = summary.SubTotal.GetAmount(percentResult.Value);
                    summary.SubTotalAfterDisocunt -= (summary.DiscountCodeAmount ?? 0);
                }
            }

            var taxResult = await _paymentDataService.GetTaxRate();
            if (taxResult.IsSuccess)
            {
                summary.Tax = summary.SubTotalAfterDisocunt.GetAmount(taxResult.Value);
                summary.TaxRate = taxResult.Value;
            }
            summary.Total = summary.SubTotal -
                (summary.TotalDiscount ?? 0) -
                (summary.DiscountCodeAmount ?? 0) +
                (summary.Tax ?? 0);

            return summary;
        }

        public async Task<Result> AddCoupon(ClientSideAddCouponToCartViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.UserId == userId &&
            n.DiscountCode == model.Code &&
            n.Status == CartStatusEnum.PendingPayment))
                return Result.Failure(Messages.AlreadyUsedThisCouponError);

            var couponResult = await _couponService.CheckCouponIsValidForUser(model.Code);
            if (couponResult.IsFailure) return Result.Failure(couponResult.Message);

            var cart = await _dbSet.Where(n => n.UserId == userId && n.Status == CartStatusEnum.PendingPayment)
                .FirstOrDefaultAsync();
            if (cart is null) return Result.Failure(Messages.OperationFailedError);

            cart.DiscountCode = model.Code;

            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.DiscountAppliedSuccessfully);
        }

        public async Task<Result> ClientSideAddUserIdToCurrentUserCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var cart = await _dbSet.Where(n => (n.UserId == userId || n.UserIP == userIP) &&
            n.Status == CartStatusEnum.PendingPayment && (n.UserId == null || n.UserId == 0)).FirstOrDefaultAsync();
            if (cart is null) return Result.Failure(Messages.OperationFailedError);

            cart.UserId = userId;
            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<string>> ClientSideGetCurrentUserTradingViewAccountName()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<string>(Messages.OperationFailedError);

            var accountName = await _dbSet.Where(n => n.Status == CartStatusEnum.PendingPayment && n.UserId == userId)
                .Select(n => n.TradingViewAccountName).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(accountName)) return Result.Failure<string>(Messages.OperationFailedError);

            return accountName;
        }

        public async Task<Result> ClientSideUpdateCurrentUserTradingViewAccountName(ClientSideUpdateTradingViewAccountNameViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var IsAllowedResult = await _itemService.IsAnyTradingViewProductInCart();
            if (IsAllowedResult.IsFailure) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var cart = await _dbSet.Where(n => n.UserId == userId && n.Status == CartStatusEnum.PendingPayment)
                .FirstOrDefaultAsync();
            if (cart is null) return Result.Failure(Messages.OperationFailedError);

            cart.TradingViewAccountName = model.TradingViewAccountName;

            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}