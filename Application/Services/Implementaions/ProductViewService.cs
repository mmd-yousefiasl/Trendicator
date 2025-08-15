using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductViewService : IProductViewService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductView> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public ProductViewService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<ProductView>();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public async Task<Result> AddProductView(int productId)
        {
            if (productId < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            if (await _dbSet.AnyAsync(n => (n.UserId == userId || n.UserIP == userIP) && n.ProductId == productId)) return Result.Failure(Messages.OperationFailedError);

            var view = new ProductView()
            {
                ProductId = productId,
                UserIP = userIP
            };
            if (userId > 0) view.UserId = userId;

            await _dbSet.AddAsync(view);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
