using Application.Dtos.BalanceTransaction.ClientSide;
using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.AdminSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Dtos.Payment;
using Application.Dtos.UserOrderStats.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Data.Context;
using Domain.Enums.Payment;
using Domain.Enums.Products;
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
                ItemsCount = cartItems.Where(n => n.IsAvailable).Count(),
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

            var product = await _context.Products
                .AsNoTracking()
                .Where(n => n.Id == model.ProductId && n.IsAvailable && n.IsPublished)
                .FirstOrDefaultAsync();
            if (product is null)
                return Result.Failure<int>(Messages.OperationFailedError);

            var cartItem = new CartItem()
            {
                CartId = cartId.Value,
                ProductId = model.ProductId,
                OrderPrice = product.OrderPrice,
                DisplayPrice = product.DisplayPrice,
                Title = product.Title,
                ProductType = product.Type,
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

        public async Task<Result<ClientSideProceedToCheckoutDto>> ClientSideProceedToCheckout(bool useWallet)
        {
            var orderResult = await ClientSideGetCart();
            if (orderResult.IsFailure)
                return Result.Failure<ClientSideProceedToCheckoutDto>(orderResult.Message!);

            if (orderResult.Value.HaveUnavailableProduct)
                return Result.Failure<ClientSideProceedToCheckoutDto>(Messages.RemoveUnavailableItemsBeforeProceedToCheckout);

            decimal totalOrder = orderResult.Value.PayAmount;
            if (totalOrder < 1)
                return Result.Failure<ClientSideProceedToCheckoutDto>(Messages.OperationFailedError);

            var userBalance = await _userService.GetCurrentUserBalance();

            decimal payAmount = useWallet ? totalOrder - userBalance : totalOrder;

            if (useWallet && userBalance >= totalOrder)
            {
                return Result.Success(new ClientSideProceedToCheckoutDto
                {
                    IsDone = true
                });
            }

            if (payAmount < 1)
                return Result.Failure<ClientSideProceedToCheckoutDto>(Messages.OperationFailedError);

            var paymentResult = await _paymentService.Request(new PaymentRequestViewModel
            {
                Amount = payAmount * 10,
                CallBackUrl = SiteTools.SiteUrl + SiteTools.CallBackURL
            });

            if (paymentResult.IsFailure || paymentResult.Value.result != 100)
                return Result.Failure<ClientSideProceedToCheckoutDto>(paymentResult.Message!);

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
                return Result.Failure<ClientSideProceedToCheckoutDto>(transactionResult.Message!);

            return Result.Success(new ClientSideProceedToCheckoutDto
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
            var userId = _httpContextAccessor.HttpContext!.User.GetUserId();

            if (userId < 1)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

            await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));

            var orderResult = await ClientSideGetCart();
            if (orderResult.IsFailure || orderResult.Value.PayAmount < 1)
                return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

            var strategy = _context.Database.CreateExecutionStrategy();

            Cart cart = null;

            var result = await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();

                var transactionResult = await _userService.WithdrawCurrentUserCart(
                    orderResult.Value.PayAmount, orderResult.Value.Id);

                if (transactionResult.IsFailure)
                    return Result.Failure<ClientSidePaymentResultDto>(transactionResult.Message!);

                cart = await _context.Carts.FindAsync(orderResult.Value.Id);
                if (cart is null)
                    return Result.Failure<ClientSidePaymentResultDto>(Messages.OperationFailedError);

                cart.TotalPaid = orderResult.Value.PayAmount;
                cart.TotalDiscount = (orderResult.Value.DiscountCodeDiscount) + (orderResult.Value.ProductsDiscount);
                cart.PaidDate = DateTime.UtcNow;
                cart.Status = CartStatusEnum.Paid;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Result.Success(new ClientSidePaymentResultDto
                {
                    CartId = cart.Id,
                    CurrentDate = cart.PaidDate.Value
                });
            });

            if (result.IsFailure)
                return Result.Failure<ClientSidePaymentResultDto>(result.Message!);

            var productIds = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .Select(ci => ci.ProductId)
                .ToListAsync();

            CartPaidEvent CartPaidEvent = new CartPaidEvent(
                cart.Id,
                cart.Status,
                cart.TotalPaid,
                cart.TotalDiscount,
                cart.DiscountCode,
                cart.PaidDate,
                cart.UserId,
                cart.VisitorId,
                productIds);

            await _mediator.PublishEvent(CartPaidEvent);
            await _cacheManager.RemoveByKeyAsync(CacheKeys.CartCacheKey(userId.ToString()));

            return result.Value;
        }

        public async Task<Result<AdminSideCartDetailsDto>> AdminSideGetCartDetails(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideCartDetailsDto>(Messages.OperationFailedError);

            var cart = await _context
                .Carts
                .AsNoTracking()
                .Where(c => c.Id == id && c.Status == CartStatusEnum.Paid)
                .Select(c => new AdminSideCartDetailsDto
                {
                    Id = c.Id,
                    PaidDate = c.PaidDate ?? DateTime.UtcNow,
                    TotalDiscount = c.TotalDiscount ?? 0,
                    TotalPaid = c.TotalPaid ?? 0,
                    DiscountCode = c.DiscountCode ?? "",
                    Products = c.Items
                        .Select(i => new AdminSideCartItemDto
                        {
                            Title = i.Title,
                            OrderPrice = i.OrderPrice,
                            DisplayPrice = i.DisplayPrice
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (cart is null)
                return Result.Failure<AdminSideCartDetailsDto>(Messages.OperationFailedError);

            return cart;
        }

        public async Task<Result<AdminSideFilterCartsDto>> AdminSideFilterCarts(AdminSideFilterCartsDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterCartsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();

            conditions.Add(n => n.Status == CartStatusEnum.Paid);

            if (filter.Status is not null)
            {
                conditions.Add(n => n.Status == filter.Status);
            }

            IQueryable<Cart> query = _context.Carts;

            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Cart>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideCartDto()
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

        public async Task<Result<ClientSideFilterCartsDto>> ClientSideFilterCarts(ClientSideFilterCartsDto filter)
        {
            if (filter is null)
                return Result.Failure<ClientSideFilterCartsDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return Result.Failure<ClientSideFilterCartsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Cart>();

            conditions.Add(n => n.UserId == userId);
            conditions.Add(n => n.Status == CartStatusEnum.Paid);

            IQueryable<Cart> query = _context.Carts;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Cart>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideOrderDto()
            {
                Id = n.Id,
                Status = n.Status,
                PaidDate = n.PaidDate,
                TotalDiscount = n.TotalDiscount,
                TotalPaid = n.TotalPaid
            }));

            return filter;
        }

        public async Task<Result<ClientSideCartDetailsDto>> ClientSideGetCartDetails(int id)
        {
            if (id < 1)
                return Result.Failure<ClientSideCartDetailsDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure<ClientSideCartDetailsDto>(Messages.OperationFailedError);

            var cart = await _context
                .Carts
                .AsNoTracking()
                .Where(c => c.Id == id && c.Status == CartStatusEnum.Paid && c.UserId == userId)
                .Select(c => new ClientSideCartDetailsDto
                {
                    Id = c.Id,
                    PaidDate = c.PaidDate ?? DateTime.UtcNow,
                    TotalDiscount = c.TotalDiscount ?? 0,
                    TotalPaid = c.TotalPaid ?? 0,
                    DiscountCode = c.DiscountCode ?? "",
                    Products = c.Items
                        .Select(i => new ClientSIdeOrderItemDto
                        {
                            Title = i.Title,
                            OrderPrice = i.OrderPrice,
                            DisplayPrice = i.DisplayPrice
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (cart is null)
                return Result.Failure<ClientSideCartDetailsDto>(Messages.OperationFailedError);

            return cart;
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

        public async Task<Result<string>> ClientSideGetCurrentUserTradingViewAccountName()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<string>(Messages.OperationFailedError);

            var accountName = await _dbSet.Where(n => n.Status == CartStatusEnum.PendingPayment && n.UserId == userId)
                .FirstOrDefaultAsync();

            return "";
        }

        public async Task<int> OrdersCount()
        {
            return 999;
            return await _dbSet.Where(n => n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment).CountAsync();
        }

        public async Task<decimal> OrdersValue()
        {
            return 999;
            return await _dbSet.Where(n => n.Status != CartStatusEnum.PendingPayment &&
                                           n.Status != CartStatusEnum.PendingPayment)
                .SumAsync(n => (n.TotalPaid ?? 0));
        }

        public async Task<Result<ClientSideFilterMyProductsDto>> ClientSideFilterPurchasedProducts(ClientSideFilterMyProductsDto filter)
        {
            if (filter is null)
                return Result.Failure<ClientSideFilterMyProductsDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return Result.Failure<ClientSideFilterMyProductsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<CartItem>();

            conditions.Add(n => n.Cart.UserId == userId);
            conditions.Add(n => n.Cart.Status == CartStatusEnum.Paid);

            if (filter.CartId > 0)
            {
                conditions.Add(n => n.CartId == filter.CartId);
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.ProductType == filter.Type);
            }

            IQueryable<CartItem> query = _context.CartItems;

            query = query.OrderByDescending(n => n.CartId);

            foreach (var item in conditions ?? new FilterConditions<CartItem>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideMyProductDto()
            {
                PaidDate = n.Cart.PaidDate ?? DateTime.UtcNow,
                CartId = n.CartId,
                ProductId = n.ProductId,
                ProductTitle = n.Title,
                Type = n.ProductType,
            }));

            return filter;
        }

        public async Task<Result<ClientSideDownloadFileDto>> ClientSideGetFileInfo(int cartId, int productId)
        {
            if (cartId < 1 || productId < 1)
                return Result.Failure<ClientSideDownloadFileDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return Result.Failure<ClientSideDownloadFileDto>(Messages.OperationFailedError);

            var file = await _context.CartItems
                .AsNoTracking()
                .Where(n => n.CartId == cartId &&
                n.ProductId == productId &&
                n.Cart.UserId == userId &&
                n.Cart.Status == CartStatusEnum.Paid &&
                n.ProductType == ProductTypeEnum.IndicatorFile)
                .Select(n => new ClientSideDownloadFileDto()
                {
                    FileName = n.Product.Title,
                    FileAddress = n.Product.IndicatorFileName ?? ""
                }).FirstOrDefaultAsync();

            if (file is null || string.IsNullOrWhiteSpace(file.FileAddress))
                return Result.Failure<ClientSideDownloadFileDto>(Messages.OperationFailedError);

            file.FileAddress = Directory.GetCurrentDirectory() + "/SensitiveFiles" + SiteTools.ProductIndicator + file.FileAddress;

            return file;
        }

        public async Task<Result<string>> ClientSideGetMetaTraderLink(int cartId, int productId)
        {
            if (cartId < 1 || productId < 1)
                return Result.Failure<string>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return Result.Failure<string>(Messages.OperationFailedError);

            var link = await _context.CartItems
                .AsNoTracking()
                .Where(n => n.CartId == cartId &&
                n.ProductId == productId &&
                n.Cart.UserId == userId &&
                n.Cart.Status == CartStatusEnum.Paid &&
                n.ProductType == ProductTypeEnum.MetaTraderLink)
                .Select(n => n.Product.MetaTraderLink)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(link))
                return Result.Failure<string>(Messages.OperationFailedError);

            return link;
        }

        #region User Order Stats
        public async Task<Result<ClientSideUserOrderStatsDto>> ClientSideGetUserOrderStats()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure<ClientSideUserOrderStatsDto>(Messages.OperationFailedError);

            var cacheKey = CacheKeys.UserOrderStatsCacheKey(userId.ToString());

            // 1) try cache → fallback to DB
            var stats = await _cacheManager.GetAsync(cacheKey, async () =>
                await _context.UserOrderStats
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.UserId == userId));

            // 2) no row, or row is flagged → recompute
            if (stats is null || stats.NeedToUpdate)
            {
                var claimed = false;

                if (stats is null)
                {
                    // insert a stub, then recompute
                    var fresh = new UserOrderStats { UserId = userId, NeedToUpdate = true };
                    _context.UserOrderStats.Add(fresh);
                    try
                    {
                        await _context.SaveChangesAsync();
                        claimed = true;
                    }
                    catch (DbUpdateException)
                    {
                        // another request inserted it in the meantime
                        _context.Entry(fresh).State = EntityState.Detached;
                        claimed = await _context.UserOrderStats
                            .Where(n => n.UserId == userId && n.NeedToUpdate)
                            .ExecuteUpdateAsync(s => s.SetProperty(n => n.NeedToUpdate, false)) == 1;
                    }
                }
                else
                {
                    // atomic claim: only one caller flips the flag and recomputes
                    claimed = await _context.UserOrderStats
                        .Where(n => n.UserId == userId && n.NeedToUpdate)
                        .ExecuteUpdateAsync(s => s.SetProperty(n => n.NeedToUpdate, false)) == 1;
                }

                if (claimed)
                {
                    // totals per order
                    var agg = await _context.Carts
                        .Where(n => n.UserId == userId && n.Status == CartStatusEnum.Paid)
                        .GroupBy(n => 1)
                        .Select(g => new
                        {
                            OrdersCount = g.Count(),
                            TotalPaid = g.Sum(c => c.TotalPaid ?? 0),
                            TotalDiscount = g.Sum(c => c.TotalDiscount ?? 0),
                        })
                        .FirstOrDefaultAsync();

                    // distinct products ever bought — adjust nav name to your schema
                    var purchasedProductsCount = await _context.Carts
                        .Where(n => n.UserId == userId && n.Status == CartStatusEnum.Paid)
                        .SelectMany(n => n.Items)
                        .Select(d => d.ProductId)
                        .CountAsync();

                    await _context.UserOrderStats
                        .Where(n => n.UserId == userId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(n => n.OrdersCount, agg?.OrdersCount ?? 0)
                            .SetProperty(n => n.TotalPaid, agg?.TotalPaid ?? 0)
                            .SetProperty(n => n.TotalDiscount, agg?.TotalDiscount ?? 0)
                            .SetProperty(n => n.PurchasedProductsCount, purchasedProductsCount));
                }

                // drop stale cache; the next call repopulates from DB
                await _cacheManager.RemoveByKeyAsync(cacheKey);

                stats = await _context.UserOrderStats
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.UserId == userId);
            }

            var dto = new ClientSideUserOrderStatsDto
            {
                OrdersCount = stats?.OrdersCount ?? 0,
                TotalPaid = stats?.TotalPaid ?? 0,
                TotalDiscount = stats?.TotalDiscount ?? 0,
                PurchasedProductsCount = stats?.PurchasedProductsCount ?? 0,
            };

            return Result.Success(dto);
        }

        public async Task<Result> UserOrderStatsNeedUpdate(int userId)
        {
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var exists = await _context.UserOrderStats
                .AsNoTracking().AnyAsync(n => n.UserId == userId);

            if (!exists)
            {
                var stats = new UserOrderStats()
                {
                    UserId = userId,
                    NeedToUpdate = true,
                };

                try
                {
                    _context.UserOrderStats.Add(stats);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    _context.Entry(stats).State = EntityState.Detached;
                }
            }

            var updated = await _context.UserOrderStats
                .Where(n => n.UserId == userId)
                .ExecuteUpdateAsync(n => n.SetProperty(c => c.NeedToUpdate, true));

            if (updated != 1)
                return Result.Failure(Messages.OperationFailedError);

            await _cacheManager.RemoveByKeyAsync(CacheKeys.UserOrderStatsCacheKey(userId.ToString()));

            return Result.Success();
        }
        #endregion
    }
}