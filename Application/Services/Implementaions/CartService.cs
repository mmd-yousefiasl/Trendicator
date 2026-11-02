using Application.Dtos.BalanceTransaction.ClientSide;
using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Dtos.Payment;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Payment;
using Domain.Enums.Shopping;
using Domain.Enums.Users;
using Domain.Events.Shopping;
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
        private readonly IVisitorService _visitorService;

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
            IBalanceTransactionService transactionService,
            IVisitorService visitorService)
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
            _visitorService = visitorService;
        }

        #endregion

        public async Task<Result> AddToCart(int optionId)
        {
            if (optionId < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var productIdResult = await _productService.ClientSideGetProductIdByOptionId(optionId);
            if (productIdResult.IsFailure) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n =>
                    (n.UserId == userId || n.UserIP == userIP) && n.Status == CartStatusEnum.PendingPayment))
            {
                var cartId = await _dbSet.Where(n => (n.UserId == userId || n.UserIP == userIP) &&
                                                     n.Status == CartStatusEnum.PendingPayment).Select(n => n.Id)
                    .FirstOrDefaultAsync();
                if (cartId < 1) return Result.Failure(Messages.OperationFailedError);

                var itemResult = await _itemService.ClientSideCreateNewCartItem(
                    new ClientSideCreateNewCartItemViewModel()
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
                    UserIP = userIP,
                    UserId = userId,
                    Status = CartStatusEnum.PendingPayment,
                    Items = new List<CartItem>()
                };

                cart.Items.Add(new CartItem()
                {
                    CartId = cart.Id,
                    ProductId = productIdResult.Value,
                });

                await _dbSet.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return Result.Success(Messages.AddedToCartSuccessfully);
        }

        public async Task<Result<List<ClientSideCartViewModel>>> ClientSideGetCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var visitorId = _visitorService.GetOrCreateVisitorId();

            var items = await _dbSet
                .Where(cart => (cart.UserId == userId || cart.VisitorId == visitorId) && cart.Status == CartStatusEnum.PendingPayment)
                .SelectMany(cart => cart.Items)
                .Select(item => new
                {
                    item.Product.IsAvailable,
                    item.Product.BannerFileName,
                    item.Product.Title,
                    item.Product.Slug,
                })
                .Select(x => new ClientSideCartViewModel
                {
                    FileName = x.BannerFileName,
                    IsAvailable = x.IsAvailable,
                    Title = x.Title,
                    Slug = x.Slug,
                })
                .AsNoTracking()
                .ToListAsync();
            if (!items.Any()) return Result.Failure<List<ClientSideCartViewModel>>(Messages.OperationFailedError);

            return items;
        }

        public async Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary()
        {
            var cartItemsResult = await ClientSideGetCart();
            if (cartItemsResult.IsFailure)
                return Result.Failure<ClientSideCartSummaryViewModel>(cartItemsResult.Message);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var cartItems = cartItemsResult.Value;

            ClientSideCartSummaryViewModel summary = new ClientSideCartSummaryViewModel();
            summary.SubTotal = cartItems.Where(n => n.IsAvailable).Sum(n => n.Price);
            summary.TotalDiscount =
                cartItems.Where(n => n.OrderPrice > 0 && n.IsAvailable).Sum(n => n.Price - n.OrderPrice);
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
                                               n.Status == CartStatusEnum.PendingPayment &&
                                               (n.UserId == null || n.UserId == 0)).FirstOrDefaultAsync();
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
                .FirstOrDefaultAsync();

            return "";
        }

        public async Task<Result> ClientSideUpdateCurrentUserTradingViewAccountName(
            ClientSideUpdateTradingViewAccountNameViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var IsAllowedResult = await _itemService.IsAnyTradingViewProductInCart();
            if (IsAllowedResult.IsFailure) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var cart = await _dbSet.Where(n => n.UserId == userId && n.Status == CartStatusEnum.PendingPayment)
                .FirstOrDefaultAsync();
            if (cart is null) return Result.Failure(Messages.OperationFailedError);

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
                                                            n.Status == CartStatusEnum.PendingPayment).AnyAsync();

            if (IsAnyAccountName) return Result.Success();

            return Result.Failure(Messages.AddYourTradingViewAccountNameError);
        }

        public async Task<Result<ClientSideProceedToCheckoutViewModel>> ClientSideProceedToCheckout(bool useWallet)
        {
            var orderResult = await ClientSideCartSummary();
            if (orderResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(orderResult.Message!);

            decimal totalOrder = orderResult.Value.Total;
            if (totalOrder < 1)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(Messages.OperationFailedError);

            var userBalance = await _userService.GetCurrentUserBalance();
            if (userBalance.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(userBalance.Message!);

            decimal payAmount = useWallet ? totalOrder - userBalance.Value : totalOrder;

            if (useWallet && userBalance.Value >= totalOrder)
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
                Amount = payAmount * 10,
                CallBackUrl = SiteTools.SiteUrl + SiteTools.CallBackURL
            });

            if (paymentResult.IsFailure || paymentResult.Value.result != 100)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(paymentResult.Message);

            var transactionResult = await _transactionService.AddUnVerifiedTransaction(
                new ClientSideCreateNewTransactionViewModel()
                {
                    Amount = payAmount,
                    TransId = paymentResult.Value.trackId.ToString(),
                    IsSuccess = false,
                    Type = BalanceTransactionTypeEnum.Credit,
                    Description = "افزایش موجودی کیف پول از طریق درگاه پرداخت به منظور پردازش سفارش"
                });
            if (transactionResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(transactionResult.Message);

            return Result.Success(new ClientSideProceedToCheckoutViewModel
            {
                IsDone = false,
                TransId = paymentResult.Value.trackId.ToString()
            });
        }

        public async Task<Result> ClientSideCheckout(PaymentResultViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            if (model.success != 1 || model.status != PaymentResultStatusEnum.PaidNotVerified)
                return Result.Failure(Messages.PaymentFailedError);

            var verifyModel = new VerifyPaymentViewModel
            {
                TrackId = model.trackId,
            };

            var paymentResult = await _paymentService.VerifyPayment(verifyModel);
            if (paymentResult.IsFailure || paymentResult.Value.status != VerifyPaymentResultStatusEnum.PaidAndVerified)
                return Result.Failure(Messages.PaymentFailedError);

            var verifyTransaction = await _transactionService.VerifyTransaction(model.trackId.ToString());
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
            if (itemResult.IsSuccess) cart.Status = CartStatusEnum.PendingPayment;
            else cart.Status = CartStatusEnum.PendingPayment;

            cart.TotalPaid = order.Total;
            cart.TotalDiscount = (order.TotalDiscount ?? 0) + (order.DiscountCodeAmount ?? 0);
            cart.PaidDate = DateTime.UtcNow;

            _dbSet.Update(cart);
            await _context.SaveChangesAsync();

            CartUpdatedEvent CartUpdatedEvent = new CartUpdatedEvent(
                cart.Id,
                cart.UserIP,
                cart.UserId,
                cart.Status,
                true,
                cart.TotalPaid,
                cart.TotalDiscount,
                cart.DiscountCode,
                1,
                1,
                cart.PaidDate);

            await _mediator.PublishEvent(CartUpdatedEvent);
            return cart.Id;
        }

        public async Task<Result<AdminSideFilterCartsViewModel>> AdminSideFilterCarts(
            AdminSideFilterCartsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterCartsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();
            conditions.Add(n => n.Status != CartStatusEnum.PendingPayment);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.UserIP == filter.Search ||
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
            if (status is null)
                return Result.Failure<AdminSideUpdateCartStatusViewModel>(Messages.OperationFailedError);

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

            var cartEntity = await _dbSet.Include(n => n.Items).ThenInclude(i => i.Product)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(n => n.Id == id && n.Status != CartStatusEnum.PendingPayment);

            if (cartEntity is null)
                return Result.Failure<AdminSideCartDetailsViewModel>(Messages.OperationFailedError);

            var cart = new AdminSideCartDetailsViewModel
            {
                Id = cartEntity.Id,
                PaidDate = cartEntity.PaidDate ?? DateTime.UtcNow,
                TotalDiscount = cartEntity.TotalDiscount ?? 0,
                TotalPaid = cartEntity.TotalPaid ?? 0,
                Products = cartEntity.Items?.Select(i =>
                {
                    return new AdminSideCartItemViewModel
                    {
                        Title = i.Product.Title,
                    };
                }).ToList() ?? new List<AdminSideCartItemViewModel>()
            };

            return cart;
        }

        public async Task<Result<ClientSideFilterCartsViewModel>> ClientSideFilterCarts(
            ClientSideFilterCartsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterCartsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterCartsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();

            conditions.Add(n => n.UserId == userId);
            conditions.Add(n => n.Status != CartStatusEnum.PendingPayment);

            IQueryable<Cart> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Cart>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideOrderViewModel()
            {
                Id = n.Id,
                Status = n.Status,
                DiscountCode = n.DiscountCode,
                PaidDate = n.PaidDate,
                TotalDiscount = n.TotalDiscount,
                TotalPaid = n.TotalPaid
            }));

            return filter;
        }

        public async Task<Result<ClientSideCartDetailsViewModel>> ClientSideGetCartDetails(int id)
        {
            if (id < 1)
                return Result.Failure<ClientSideCartDetailsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideCartDetailsViewModel>(Messages.OperationFailedError);

            var cartEntity = await _dbSet
                .Include(n => n.Items)
                .ThenInclude(i => i.Product)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(n => n.Id == id &&
                                          n.Status != CartStatusEnum.PendingPayment &&
                                          n.UserId == userId);

            if (cartEntity is null)
                return Result.Failure<ClientSideCartDetailsViewModel>(Messages.OperationFailedError);

            var cart = new ClientSideCartDetailsViewModel
            {
                Id = cartEntity.Id,
                PaidDate = cartEntity.PaidDate ?? DateTime.UtcNow,
                TotalDiscount = cartEntity.TotalDiscount ?? 0,
                TotalPaid = cartEntity.TotalPaid ?? 0,
                Products = cartEntity.Items?.Select(i =>
                {
                    return new ClientSIdeCartItemViewModel
                    {
                        Title = i.Product.Title,
                    };
                }).ToList() ?? new List<ClientSIdeCartItemViewModel>()
            };

            return cart;
        }

        public async Task<int> OrdersCount()
        {
            return await _dbSet.Where(n => n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment).CountAsync();
        }

        public async Task<decimal> OrdersValue()
        {
            return await _dbSet.Where(n => n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment)
                .SumAsync(n => (n.TotalPaid ?? 0));
        }

        public async Task<int> MyOrdersCount()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return 0;

            return await _dbSet.Where(n => n.UserId == userId && n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment).CountAsync();
        }

        public async Task<decimal> MyTotalPurchesAmount()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return 0;

            return await _dbSet.Where(n => n.UserId == userId && n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment).SumAsync(n => (n.TotalPaid ?? 0));
        }
    }
}