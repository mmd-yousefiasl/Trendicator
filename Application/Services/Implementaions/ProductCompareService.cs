using Application.Dtos.ProductCompare.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductCompareService : IProductCompareService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly DbSet<ProductCompare> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public ProductCompareService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<ProductCompare>();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public async Task<Result> AddToCompare(int productId)
        {
            if (productId < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            if (await _dbSet.AnyAsync(n => (n.UserId == userId || n.UserIP == userIP) && n.ProductId == productId)) return Result.Failure(Messages.OperationFailedError);

            var compare = new ProductCompare()
            {
                ProductId = productId,
                UserIP = userIP,
            };
            if (userId > 0) compare.UserId = userId;

            await _dbSet.AddAsync(compare);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<ClientSideFilterProductComareViewModel>> ClientSideFilterProductCompare(ClientSideFilterProductComareViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterProductComareViewModel>(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var conditions = Filter.GenerateConditions<ProductCompare>();
            conditions.Add(n => n.UserIP == userIP);
            if (userId > 0) conditions.Add(n => n.UserIP == userIP || n.UserId == userId);

            IQueryable<ProductCompare> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductCompare>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideProductCompareViewModel()
            {
                Id = n.Id,
                ProductId = n.ProductId,
                Title = n.Product.Title,
                Slug = n.Product.Slug,
                ShortDescription = n.Product.ShortDescription,
                CategoryTitle = n.Product.Category.Title,
                FileName = n.Product.BannerFileName,
                IsAvailable = n.Product.IsAvailable,
                Price = n.Product.DisplayPrice
            }));

            return filter;
        }

        public async Task<Result> DeleteCompare(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            IQueryable<ProductCompare> query = _dbSet;
            query = query.Where(n => n.Id == id);
            query = query.Where(n => n.UserIP == userIP);
            if (userId > 0) query = query.Where(n => n.UserId == userId || n.UserIP == userIP);

            var compare = await query.FirstOrDefaultAsync();
            if (compare is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(compare);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.SuccessfullyDone);
        }

        public async Task<Result<List<int>>> GetCurrentUserCompare()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var productIds = await _dbSet.Where(n => n.UserId == userId || n.UserIP == userIP).Select(n => n.ProductId)
                .ToListAsync();
            if (!productIds?.Any() ?? true) return Result.Failure<List<int>>(Messages.OperationFailedError);

            return productIds;
        }

        public async Task<bool> IsInUserCompare(int productId)
        {
            if (productId < 1) return false;

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            return await _dbSet.AnyAsync(n => n.ProductId == productId && (n.UserId == userId || n.UserIP == userIP));
        }

        public async Task<Result> RemoveFromCompare(int productId)
        {
            if (productId < 1) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            IQueryable<ProductCompare> query = _dbSet;
            query = query.Where(n => n.ProductId == productId);
            query = query.Where(n => n.UserIP == userIP);
            if (userId > 0) query = query.Where(n => n.UserId == userId || n.UserIP == userIP);

            var compare = await query.FirstOrDefaultAsync();
            if (compare is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(compare);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
