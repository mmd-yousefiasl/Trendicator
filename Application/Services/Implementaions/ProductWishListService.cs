using Application.Dtos.ProductWishList.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductWishListService : IProductWishListService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductWishList> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public ProductWishListService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<ProductWishList>();
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion

        public async Task<bool> IsInUserWishList(int productId)
        {
            if (productId < 1) return false;

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            return await _dbSet.AnyAsync(n => (n.UserId == userId || n.UserIP == userIP) && n.ProductId == productId);
        }

        public async Task<Result> AddProductToWishList(int productId)
        {
            if (productId < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            if (await _dbSet.AnyAsync(n => (n.UserIP == userIP || n.UserId == userId) && n.ProductId == productId))
                return Result.Failure(Messages.OperationFailedError);

            ProductWishList wishList = new ProductWishList()
            {
                ProductId = productId,
                UserIP = userIP
            };
            if (userId > 0) wishList.UserId = userId;

            await _dbSet.AddAsync(wishList);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RemoveProductFromWishList(int productId)
        {
            if (productId < 1) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            IQueryable<ProductWishList> query = _dbSet;
            query = query.Where(n => n.ProductId == productId);
            query = query.Where(n => n.UserIP == userIP);
            if (userId > 0) query = query.Where(n => n.UserId == userId || n.UserIP == userIP);

            var wishList = await query.FirstOrDefaultAsync();
            if (wishList is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(wishList);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.SuccessfullyDone);
        }

        public async Task<Result<ClientSideFilterProductWishListViewModel>> ClientSideFilterWishList(ClientSideFilterProductWishListViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterProductWishListViewModel>(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var conditions = Filter.GenerateConditions<ProductWishList>();
            conditions.Add(n => n.UserIP == userIP);
            if (userId > 0) conditions.Add(n => n.UserId == userId || n.UserIP == userIP);

            IQueryable<ProductWishList> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductWishList>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideProductWishListViewModel()
            {
                Id = n.Id,
                Title = n.Product.Title,
                FileName = n.Product.BannerFileName,
                Slug = n.Product.Slug,
                Price = n.Product.DisplayPrice,
                ProductId = n.ProductId,
                IsAvailable = n.Product.IsAvailable
            }));

            return filter;
        }

        public async Task<Result<List<int>>> GetCurrentUserWishListIds()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            IQueryable<ProductWishList> query = _dbSet;
            query = query.Where(n => n.UserIP == userIP);
            if (userId > 0) query = query.Where(n => n.UserId == userId || n.UserIP == userIP);

            var ids = await query.Select(n => n.ProductId).ToListAsync();
            if (!ids?.Any() ?? true) return Result.Failure<List<int>>(Messages.OperationFailedError);

            return ids;
        }

        public async Task<Result> DeleteWishList(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            IQueryable<ProductWishList> query = _dbSet;
            query = query.Where(n => n.Id == id);
            query = query.Where(n => n.UserIP == userIP);
            if (userId > 0) query = query.Where(n => n.UserId == userId || n.UserIP == userIP);

            var wishList = await query.FirstOrDefaultAsync();
            if (wishList is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(wishList);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.SuccessfullyDone);
        }
    }
}
