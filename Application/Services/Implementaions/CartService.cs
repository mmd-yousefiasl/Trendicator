using Application.Dtos.BalanceTransaction.ClientSide;
using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Dtos.Payment;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
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
using Npgsql;

namespace Application.Services.Implementaions
{
    public class CartService : ICartService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Cart> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICartItemService _itemService;
        private readonly IPaymentDataService _paymentDataService;
        private readonly ICouponService _couponService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IBalanceTransactionService _transactionService;
        private readonly IVisitorService _visitorService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CartService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IHttpContextAccessor httpContextAccessor,
            ICartItemService itemService,
            IPaymentDataService paymentDataService,
            ICouponService couponService,
            IPaymentService paymentService,
            IUserService userService,
            IBalanceTransactionService transactionService,
            IVisitorService visitorService,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
            _paymentDataService = paymentDataService;
            _couponService = couponService;
            _paymentService = paymentService;
            _userService = userService;
            _transactionService = transactionService;
            _visitorService = visitorService;
            _cacheManager = cacheManager;
        }

        #endregion

        public async Task<Result<ClientSideCartSnapshotDto>> ClientSideGetCartSnapshot()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            string cacheKey;
            int? userFilter = null;
            string? visitorFilter = null;

            if (userId > 0)
            {
                userFilter = userId;
                cacheKey = CacheKeys.CartCacheKey(userId.ToString());
            }
            else
            {
                visitorFilter = _visitorService.GetOrCreateVisitorId();
                cacheKey = CacheKeys.CartCacheKey(visitorFilter);
            }

            var cartSnapshot = await _cacheManager.GetAsync(cacheKey, async () =>
            {
                var query = _context.Carts
                    .AsNoTracking()
                    .Where(c => c.Status == CartStatusEnum.PendingPayment);

                query = userFilter.HasValue
                    ? query.Where(c => c.UserId == userFilter.Value)
                    : query.Where(c => c.VisitorId == visitorFilter);

                var snapshot = await query
                    .Select(n => new ClientSideCartSnapshotDto
                    {
                        CartId = n.Id,
                        DiscountCode = n.DiscountCode,
                        ProductIds = n.Items.Select(i => i.ProductId).ToList()
                    })
                    .FirstOrDefaultAsync();

                // Never return null → the "no cart yet" case is cached too.
                return snapshot ?? new ClientSideCartSnapshotDto
                {
                    CartId = 0,
                    DiscountCode = null,
                    ProductIds = new List<int>()
                };
            }, TimeSpan.FromMinutes(10));

            if (cartSnapshot?.CartId == 0)
                return Result.Failure<ClientSideCartSnapshotDto>(Messages.OperationFailedError);

            return cartSnapshot;
        }

        public async Task<Result<int>> ClientSideCreateNewCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var cart = new Cart { Status = CartStatusEnum.PendingPayment };
            string? cartCacheKey = null;

            if (userId > 0)
            {
                cart.UserId = userId;
                cartCacheKey = CacheKeys.CartCacheKey(userId.ToString());
            }
            else
            {
                cart.VisitorId = _visitorService.GetOrCreateVisitorId();
                cartCacheKey = CacheKeys.CartCacheKey(cart.VisitorId);
            }

            try
            {
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                return cart.Id;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
            {
                _context.Entry(cart).State = EntityState.Detached;

                var cartId = await _context.Carts
                    .Where(n => n.Status == CartStatusEnum.PendingPayment && (userId > 0 ? n.UserId == userId : n.VisitorId == cart.VisitorId))
                    .AsNoTracking()
                    .Select(n => n.Id)
                    .FirstOrDefaultAsync();

                if (cartId < 1)
                    return Result.Failure<int>(Messages.OperationFailedError);

                return cartId;
            }
            finally
            {
                await _cacheManager.RemoveByKeyAsync(cartCacheKey);
            }
        }

        public async Task<List<int>> ClientSideGetCartProductIds()
        {
            var cartResult = await ClientSideGetCartSnapshot();
            if (cartResult.IsFailure)
                return new List<int>();

            return cartResult.Value?.ProductIds ?? new List<int>();
        }

        public async Task<int> ClientSideGetCartItemsCount()
        {
            var productIds = await ClientSideGetCartProductIds();

            return productIds.Count();
        }

        public async Task<Result<ClientSideCartDto>> ClientSideGetCart()
        {
            var cartSnapShot = await ClientSideGetCartSnapshot();
            if (cartSnapShot.IsFailure || !cartSnapShot.Value.ProductIds.Any())
                return new ClientSideCartDto();

            var cartItems = await _context.Products
                .AsNoTracking()
                .Where(n => cartSnapShot.Value.ProductIds.Contains(n.Id))
                .Select(n => new ClientSideCartItemDto()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    Hint = n.Hint,
                    DisplayPrice = n.DisplayPrice,
                    OrderPrice = n.OrderPrice,
                    FileName = n.BannerFileName,
                    IsAvailable = n.IsAvailable
                }).ToListAsync();

            var cart = new ClientSideCartDto()
            {
                Id = cartSnapShot.Value.CartId,
                ItemsCount = cartItems.Count(),
                DisplayPriceSum = cartItems.Where(n => n.IsAvailable).Select(n => (n.DisplayPrice > 0 ? n.DisplayPrice : n.OrderPrice)).Sum(),
                OrderPriceSum = cartItems.Where(n => n.IsAvailable).Sum(n => n.OrderPrice),
                Items = cartItems,
                DiscountCode = cartSnapShot.Value.DiscountCode,
                HaveUnavailableProduct = cartItems.Any(n => !n.IsAvailable)
            };
            cart.ProductsDiscount = cart.DisplayPriceSum - cart.OrderPriceSum;

            if (!string.IsNullOrWhiteSpace(cart.DiscountCode))
            {
                var discountResult = await _couponService.ClientSideCheckCouponIsValid(cart.DiscountCode);
                if (discountResult.IsFailure)
                {
                    cart.DiscountCode = null;
                }
                else
                {
                    cart.DiscountPercent = discountResult.Value;
                    cart.DiscountCodeDiscount = cart.OrderPriceSum.GetDiscountAmount(cart.DiscountPercent);
                }
            }
            cart.PayAmount = cart.OrderPriceSum - cart.DiscountCodeDiscount;

            return cart;
        }

        public async Task<Result<int>> ClientSideAddToCart(ClientSideCartItemToggleDto model)
        {
            if (model is null || model.ProductId < 1)
                return Result.Failure<int>(Messages.OperationFailedError);

            int? cartId = null;

            var cartResult = await ClientSideGetCartSnapshot();
            if (cartResult.IsSuccess)
            {
                cartId = cartResult.Value.CartId;

                if (cartResult.Value.ProductIds.Contains(model.ProductId))
                    return Result.Success(cartResult.Value.ProductIds.Count(), Messages.AddedToCartSuccessfully);

            }
            else
            {
                var createCartResult = await ClientSideCreateNewCart();

                if (createCartResult.IsFailure)
                    return Result.Failure<int>(Messages.OperationFailedError);

                cartId = createCartResult.Value;
            }

            var cartItem = new CartItem()
            {
                CartId = cartId.Value,
                ProductId = model.ProductId
            };

            try
            {
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
            {
                _context.Entry(cartItem).State = EntityState.Detached;
            }
            finally
            {
                var userId = _httpContextAccessor.HttpContext.User.GetUserId();
                if (userId > 0)
                {
                    await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));
                }
                else
                {
                    var visitorId = _visitorService.GetOrCreateVisitorId();
                    await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(visitorId));
                }
            }

            return Result.Success(await ClientSideGetCartItemsCount(), Messages.AddedToCartSuccessfully);
        }

        public async Task<Result<int>> ClientSideRemoveFromCart(ClientSideCartItemToggleDto model)
        {
            if (model is null || model.ProductId < 1)
                return Result.Failure<int>(Messages.OperationFailedError);

            var cartResult = await ClientSideGetCartSnapshot();

            if (cartResult.IsFailure)
                return Result.Failure<int>(cartResult.Message!);

            if (!cartResult.Value.ProductIds.Contains(model.ProductId))
                return Result.Success(cartResult.Value.ProductIds.Count(), Messages.RemovedFromCartSuccessfully);

            var deleted = await _context.CartItems
                .Where(ci => ci.CartId == cartResult.Value.CartId && ci.ProductId == model.ProductId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                return Result.Failure<int>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId > 0)
            {
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));
            }
            else
            {
                var visitorId = _visitorService.GetOrCreateVisitorId();
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(visitorId));
            }

            return Result.Success(await ClientSideGetCartItemsCount(), Messages.RemovedFromCartSuccessfully);
        }

        public async Task<Result> ClientSideUpdateCartAfterUserLoggedIn(int userId)
        {
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var visitorCart = await ClientSideGetCartSnapshot();
            if (visitorCart.IsFailure)
                return Result.Success();

            if (visitorCart.Value.ProductIds.Count() > 0)
            {
                var userCarts = await _context.Carts
                    .Where(n => n.Status == CartStatusEnum.PendingPayment && n.UserId == userId)
                    .ToListAsync();

                _context.Carts.RemoveRange(userCarts);
            }

            var cart = await _context.Carts.FindAsync(visitorCart.Value.CartId);

            if (cart is null)
                return Result.Success();

            string? visitorId = cart.VisitorId;

            if (visitorCart.Value.ProductIds.Count() > 0)
            {
                cart.VisitorId = null;
                cart.UserId = userId;
            }
            else
            {
                _context.Carts.Remove(cart);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Success();   // someone else claimed it first — nothing to do
            }

            await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(visitorId ?? ""));
            await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));

            return Result.Success();
        }

        public async Task<Result> ClientSideApplyCoupon(ClientApplyCouponDto model)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Code))
                return Result.Failure(Messages.InvalidCouponCodeError);

            var cartResult = await ClientSideGetCartSnapshot();

            if (cartResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var couponResult = await _couponService.ClientSideCheckCouponIsValid(model.Code.NormalizeCouponCode());

            if (couponResult.IsFailure)
                return Result.Failure(couponResult.Message!);

            await _context.Carts
                .Where(n => n.Id == cartResult.Value.CartId)
                .ExecuteUpdateAsync(n => n.SetProperty(c => c.DiscountCode, model.Code.NormalizeCouponCode()));

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId > 0)
            {
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));
            }
            else
            {
                var visitorId = _visitorService.GetOrCreateVisitorId();
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(visitorId));
            }

            return Result.Success(Messages.DiscountAppliedSuccessfully);
        }

        public async Task<Result> ClientSideRemoveCoupon()
        {
            var cartResult = await ClientSideGetCartSnapshot();

            if (cartResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            await _context.Carts
                .Where(n => n.Id == cartResult.Value.CartId)
                .ExecuteUpdateAsync(n => n.SetProperty(c => c.DiscountCode, string.Empty));

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId > 0)
            {
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));
            }
            else
            {
                var visitorId = _visitorService.GetOrCreateVisitorId();
                await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(visitorId));
            }

            return Result.Success(Messages.DiscountRemovedSuccessfully);
        }

        public async Task<Result<ClientSideProceedToCheckoutViewModel>> ClientSideProceedToCheckout(bool useWallet)
        {
            var orderResult = await ClientSideGetCart();
            if (orderResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(orderResult.Message!);

            if (orderResult.Value.HaveUnavailableProduct)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(Messages.RemoveUnavailableItemsBeforeProceedToCheckout);

            decimal totalOrder = orderResult.Value.PayAmount;
            if (totalOrder < 1)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(Messages.OperationFailedError);

            var userBalance = await _userService.GetCurrentUserBalance();

            decimal payAmount = useWallet ? totalOrder - userBalance : totalOrder;

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
                Amount = payAmount * 10,
                CallBackUrl = SiteTools.SiteUrl + SiteTools.CallBackURL
            });

            if (paymentResult.IsFailure || paymentResult.Value.result != 100)
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(paymentResult.Message!);

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
                return Result.Failure<ClientSideProceedToCheckoutViewModel>(transactionResult.Message!);

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

        public async Task<Result<ClientSidePaymentResultDto>> ClientSideVerifyCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

            var cart = await _context.Carts
                .Where(n => n.UserId == userId && n.Status == CartStatusEnum.PendingPayment)
                .FirstOrDefaultAsync();

            if (cart is null)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

            var orderResult = await ClientSideGetCart();
            if (orderResult.IsFailure || orderResult.Value.Id != cart.Id)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);


            if (orderResult.Value.PayAmount < 1)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

            var transactionResult = await _userService.WithdrawCurrentUserCart(orderResult.Value.PayAmount, cart.Id);

            if (transactionResult.IsFailure)
                return Result.Failure<ClientSidePaymentResultDto>(transactionResult.Message!);

            cart.TotalPaid = orderResult.Value.PayAmount;
            cart.TotalDiscount = (orderResult.Value.DiscountCodeDiscount) + (orderResult.Value.ProductsDiscount);
            cart.PaidDate = DateTime.UtcNow;
            cart.Status = CartStatusEnum.Paid;

            await _context.SaveChangesAsync();

            CartUpdatedEvent CartUpdatedEvent = new CartUpdatedEvent(
                cart.Id,
                cart.Status,
                cart.TotalPaid,
                cart.TotalDiscount,
                cart.DiscountCode,
                cart.PaidDate,
                cart.UserId,
                cart.VisitorId);

            await _mediator.PublishEvent(CartUpdatedEvent);

            await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));

            return new ClientSidePaymentResultDto()
            {
                CartId = cart.Id,
                CurrentDate = cart.PaidDate.Value
            };
        }

        public async Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary()
        {
            /*            var cartItemsResult = await ClientSideGetCart();
                        if (cartItemsResult.IsFailure)
                            return Result.Failure<ClientSideCartSummaryViewModel>(cartItemsResult.Message);

                        var userId = _httpContextAccessor.HttpContext.User.GetUserId();
                        var userIP = _httpContextAccessor.HttpContext.GetUserIP();

                        var cartItems = cartItemsResult.Value;

                        ClientSideCartSummaryViewModel summary = new ClientSideCartSummaryViewModel();
                        summary.SubTotal = cartItems.Where(n => n.IsAvailable).Sum(n => n.DisplayPrice);
                        summary.TotalDiscount =
                            cartItems.Where(n => n.OrderPrice > 0 && n.IsAvailable).Sum(n => n.DisplayPrice - n.OrderPrice);
                        summary.SubTotalAfterDisocunt = summary.SubTotal - (summary.TotalDiscount ?? 0);

                        var cartCode = await _dbSet.Where(n => (n.UserId == userId) &&
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

                        return summary;*/
            return new ClientSideCartSummaryViewModel();
        }

        public async Task<Result> ClientSideAddUserIdToCurrentUserCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var cart = await _dbSet.Where(n => (n.UserId == userId) &&
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

        public async Task<Result<AdminSideFilterCartsViewModel>> AdminSideFilterCarts(
            AdminSideFilterCartsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterCartsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();
            conditions.Add(n => n.Status != CartStatusEnum.PendingPayment);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.DiscountCode.Contains(filter.Search));
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