using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.Blogs;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BlogViewService : IBlogViewService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<BlogView> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public BlogViewService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<BlogView>();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public async Task<Result> CreateNewBlogView(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            BlogView view = new BlogView()
            {
                BlogId = id,
                UserIP = userIP,
            };
            if (userId > 0)
            {
                view.UserId = userId;
            }

            await _dbSet.AddAsync(view);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
