using Application.Dtos.CartItem.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Products;
using Domain.Enums.Shopping;
using Domain.Models.Shopping;
using Domain.Shared;
using Domain.Statics;
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

        public async Task<Result<ClientSideFilterBoughtFilesViewModel>> ClientSideFilterBoughtFiles(ClientSideFilterBoughtFilesViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterBoughtFilesViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterBoughtFilesViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<CartItem>();
            conditions.Add(n => n.Cart.UserId == userId);
            conditions.Add(n => n.Cart.Status != CartStatusEnum.PendingPayment && n.Cart.Status != CartStatusEnum.Canceled);

            IQueryable<CartItem> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<CartItem>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideBoughtFileViewModel()
            {
                Id = n.Id,
                PaidDate = n.Cart.PaidDate ?? DateTime.UtcNow,
                ProductTitle = n.Product.Title,
                Type = n.Product.Type,
                Status = n.Cart.Status,
                MetaTraderLink = n.Product.MetaTraderLink,
            }));

            return filter;
        }

        public async Task<Result<ClientSideDownloadFileViewModel>> CanUserDownlaodTheCurrentFile(int id)
        {
            if (id < 1) return Result.Failure<ClientSideDownloadFileViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideDownloadFileViewModel>(Messages.OperationFailedError);

            var file = await _dbSet.Where(n => n.Id == id &&
            n.Cart.UserId == userId &&
            n.Cart.Status != CartStatusEnum.PendingPayment &&
            n.Cart.Status != CartStatusEnum.Canceled &&
            n.Product.Type == ProductTypeEnum.IndicatorFile)
                .Select(n => new ClientSideDownloadFileViewModel()
                {
                    FileName = n.Product.Title,
                    FileAddress = n.Product.IndicatorFileName,
                }).AsNoTracking().FirstOrDefaultAsync();
            if (file is null) return Result.Failure<ClientSideDownloadFileViewModel>(Messages.OperationFailedError);

            file.FileAddress = Directory.GetCurrentDirectory() + "/SensitiveFiles" + SiteTools.ProductIndicator + file.FileAddress;

            return file;
        }

        public async Task<int> SoldProductsCount()
        {
            return await _dbSet.Where(n => n.Cart.Status != CartStatusEnum.PendingPayment &&
            n.Cart.Status != CartStatusEnum.Canceled)
                .CountAsync();
        }
    }
}
