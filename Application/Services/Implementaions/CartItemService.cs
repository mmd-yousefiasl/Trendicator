using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.CartItem.ClientSide;
using Data.Context;
using Domain.Enums.Products;
using Domain.Enums.Shopping;
using Domain.Models.Shopping;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class CartItemService : ICartItemService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<CartItem> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public CartItemService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<CartItem>();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public async Task<Result> ClientSideCreateNewCartItem(ClientSideCreateNewCartItemViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.StockOptionId == model.OptionId && n.Cart.Status == CartStatusEnum.PendingPayment))
                return Result.Failure(Messages.ProductAlreadyInYourCartError);

            CartItem item = new CartItem()
            {
                CartId = model.CartId,
                ProductId = model.ProductId,
                StockOptionId = model.OptionId,
            };

            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ClientSideDeleteItemFromCart(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var item = await _dbSet.Where(n => (n.Cart.UserId == userId || n.Cart.UserIP == userIP) &&
            n.Cart.Status == CartStatusEnum.PendingPayment && n.Id == id).FirstOrDefaultAsync();
            if (item is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(item);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> CheckIfCartItemsIsAvailable()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            bool AnyUnAvailable = await _dbSet.Where(n => n.Cart.Status == CartStatusEnum.PendingPayment && n.Cart.UserId == userId &&
            n.Product.IsAvailable == false).AnyAsync();

            if (AnyUnAvailable) return Result.Failure(Messages.RemoveUnavailableItemsBeforeProceedToCheckout);

            bool AnyItem = await _dbSet.Where(n => n.Cart.Status == CartStatusEnum.PendingPayment && n.Cart.UserId == userId)
                .AnyAsync();
            if (!AnyItem) return Result.Failure(Messages.AddAProductAtLeastToCartBeforeProceedToCheckout);

            return Result.Success();
        }

        public async Task<Result> IsAnyTradingViewProductInCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            bool IsAny = await _dbSet.Where(n => n.Cart.Status == CartStatusEnum.PendingPayment &&
            n.Cart.UserId == userId &&
            n.Product.Type == ProductTypeEnum.TradingView).AnyAsync();

            if (IsAny) return Result.Success();

            return Result.Failure(Messages.OperationFailedError);
        }
    }
}
