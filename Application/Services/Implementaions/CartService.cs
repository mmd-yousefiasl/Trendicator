using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.BalanceTransaction.ClientSide;
using Application.ViewModels.Cart.AdminSide;
using Application.ViewModels.Cart.ClientSide;
using Application.ViewModels.CartItem.ClientSide;
using Application.ViewModels.Payment;
using Data.Context;
using Domain.Enums.Payment;
using Domain.Enums.Shopping;
using Domain.Enums.Users;
using Domain.Models.Shopping;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IBalanceTransactionService _transactionService;
        #endregion

        #region Ctor
        public CartService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IHttpContextAccessor httpContextAccessor,
            IProductService productService,
            ICartItemService itemService,
            IPaymentDataService paymentDataService,
            ICouponService couponService,
            IPaymentService paymentService,
            IUserService userService,
            IBalanceTransactionService transactionService)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _itemService = itemService;
            _paymentDataService = paymentDataService;
            _couponService = couponService;
            _paymentService = paymentService;
            _userService = userService;
            _transactionService = transactionService;
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

        public async Task<Result> CheckUserCanProccedToCheckout()
        {
            var itemResult = await _itemService.IsAnyTradingViewProductInCart();
            if (itemResult.IsFailure) return Result.Success();

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            bool IsAnyAccountName = await _dbSet.Where(n => n.UserId == userId &&
            n.Status == CartStatusEnum.PendingPayment && n.TradingViewAccountName != null).AnyAsync();

            if (IsAnyAccountName) return Result.Success();

            return Result.Failure(Messages.AddYourTradingViewAccountNameError);
        }

        public async Task<Result<ClientSideProceedToCheckoutViewModel>> ClientSideProceedToCheckout(bool useWallet)
        {
            var orderResult = await ClientSideCartSummary();
            if (orderResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(orderResult.Message);

            long totalOrder = orderResult.Value.Total;
            if (totalOrder < 1)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(Messages.OperationFailedError);

            var userBalance = await _userService.GetCurrentUserBalance();

            long payAmount = useWallet ? totalOrder - userBalance : totalOrder;

            if (useWallet && userBalance >= totalOrder)
            {
                return Result.Success(new ClientSideProceedToCheckoutViewModel
                {
                    IsDone = true
                });
            }

            if (payAmount < 1)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(Messages.OperationFailedError);

            var paymentResult = await _paymentService.Request(new PaymentRequestViewModel
            {
                Amount = payAmount,
                CallBackUrl = SiteTools.SiteUrl + SiteTools.CallBackURL
            });

            if (paymentResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(paymentResult.Message);

            var transactionResult = await _transactionService.AddUnVerifiedTransaction(new ClientSideCreateNewTransactionViewModel()
            {
                Amount = payAmount,
                TransId = paymentResult.Value.TransId,
                IsSuccess = false,
                Type = BalanceTransactionTypeEnum.Credit,
                Description = "افزایش موجودی کیف پول از طریق درگاه پرداخت به منظور پردازش سفارش"
            });
            if (transactionResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(transactionResult.Message);

            return Result.Success(new ClientSideProceedToCheckoutViewModel
            {
                IsDone = false,
                TransId = paymentResult.Value.TransId
            });
        }

        public async Task<Result> ClientSideCheckout(PaymentResultViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            if (model.status != PaymentResultStatusEnum.Succeed)
                return Result.Failure(Messages.PaymentFailedError);

            var amountResult = await _transactionService.GetTransactionAmountByTransId(model.transid);
            if (amountResult.IsFailure)
                return Result.Failure(amountResult.Message);

            var verifyModel = new VerifyPaymentViewModel
            {
                TransId = model.transid,
                Amount = amountResult.Value
            };

            var paymentResult = await _paymentService.VerifyPayment(verifyModel);
            if (paymentResult.IsFailure || paymentResult.Value.Code != VerifyPaymentResultStatusEnum.Success)
                return Result.Failure(Messages.PaymentFailedError);

            var verifyTransaction = await _transactionService.VerifyTransaction(model.transid);
            if (verifyTransaction.IsFailure)
                return Result.Failure(Messages.PaymentFailedError);

            return Result.Success(Messages.PaymentDoneSuccessfully);
        }

        public async Task<Result<int>> ClientSideVerifyCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<int>(Messages.OperationFailedError);

            var cart = await _dbSet.Where(n => n.UserId == userId && n.Status == CartStatusEnum.PendingPayment)
                .FirstOrDefaultAsync();
            if (cart is null) return Result.Failure<int>(Messages.OperationFailedError);

            var orderResult = await ClientSideCartSummary();
            if (orderResult.IsFailure) return Result.Failure<int>(orderResult.Message);

            var order = orderResult.Value;
            if (order.Total < 1) return Result.Failure<int>(Messages.OperationFailedError);

            var transactionResult = await _userService.WithdrawCurrentUserCart(order.Total, cart.Id);
            if (transactionResult.IsFailure) return Result.Failure<int>(transactionResult.Message);

            var itemResult = await _itemService.IsAnyTradingViewProductInCart();
            if (itemResult.IsSuccess) cart.Status = CartStatusEnum.InProgress;
            else cart.Status = CartStatusEnum.Done;

            cart.IsPaid = true;
            cart.TotalPaid = order.Total;
            cart.TotalDiscount = (order.TotalDiscount ?? 0) + (order.DiscountCodeAmount ?? 0);
            cart.SubTotal = order.SubTotal;
            cart.Tax = order.Tax ?? 0;
            cart.PaidDate = DateTime.Now;

            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            return cart.Id;
        }

        public async Task<Result<AdminSideFilterCartsViewModel>> AdminSideFilterCarts(AdminSideFilterCartsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterCartsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();
            conditions.Add(n => n.Status != CartStatusEnum.PendingPayment);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.UserIP == filter.Search ||
                n.TradingViewAccountName.Contains(filter.Search) ||
                n.DiscountCode.Contains(filter.Search));
            }

            if (filter.Status is not null)
            {
                conditions.Add(n => n.Status == filter.Status);
            }

            IQueryable<Cart> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Cart>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideCartViewModel()
            {
                Id = n.Id,
                DiscountCode = n.DiscountCode,
                Status = n.Status,
                PaidDate = n.PaidDate,
                TotalDiscount = n.TotalDiscount,
                TotalPaid = n.TotalPaid,
                UserId = n.UserId,
                UserIP = n.UserIP
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateCartStatusViewModel>> AdminSideGetStatusForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateCartStatusViewModel>(Messages.OperationFailedError);

            var status = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateCartStatusViewModel()
            {
                Id = n.Id,
                Status = n.Status,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (status is null) return Result.Failure<AdminSideUpdateCartStatusViewModel>(Messages.OperationFailedError);

            return status;
        }

        public async Task<Result> AdminSideUpdateCartStatus(AdminSideUpdateCartStatusViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var cart = await _dbSet.FindAsync(model.Id);
            if (cart is null) return Result.Failure(Messages.OperationFailedError);

            cart.Status = model.Status;

            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideCartDetailsViewModel>> AdminSideGetCartDetails(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideCartDetailsViewModel>(Messages.OperationFailedError);

            var cartEntity = await _dbSet.Include(n => n.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Options)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(n => n.Id == id && n.Status != CartStatusEnum.PendingPayment);

            if (cartEntity is null)
                return Result.Failure<AdminSideCartDetailsViewModel>(Messages.OperationFailedError);

            var cart = new AdminSideCartDetailsViewModel
            {
                Id = cartEntity.Id,
                TradingViewAccountName = cartEntity.TradingViewAccountName,
                PaidDate = cartEntity.PaidDate ?? DateTime.Now,
                SubTotal = cartEntity.SubTotal ?? 0,
                Tax = cartEntity.Tax ?? 0,
                TotalDiscount = cartEntity.TotalDiscount ?? 0,
                TotalPaid = cartEntity.TotalPaid ?? 0,
                Products = cartEntity.Items?.Select(i =>
                {
                    var option = i.Product.Options?.FirstOrDefault(o => o.Id == i.StockOptionId);
                    return new AdminSideCartItemViewModel
                    {
                        Title = i.Product.Title,
                        Option = option?.Title ?? "",
                        Price = option?.Price ?? 0,
                        OrderPrice = option?.OrderPrice ?? 0
                    };
                }).ToList() ?? new List<AdminSideCartItemViewModel>()
            };

            return cart;
        }
    }
}