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
        #endregion

        #region Ctor
        public CartService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IHttpContextAccessor httpContextAccessor,
            IProductService productService,
            ICartItemService itemService)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _itemService = itemService;
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
                    item.Product.BannerFileName,
                    item.Product.Title,
                    item.Product.Slug,
                    Option = item.Product.Options.FirstOrDefault(o => o.Id == item.StockOptionId)
                })
                .Select(x => new ClientSideCartViewModel
                {
                    Id = x.Id,
                    FileName = x.BannerFileName,
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
    }
}
