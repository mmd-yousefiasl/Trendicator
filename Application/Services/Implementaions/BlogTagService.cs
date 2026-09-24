using Application.Dtos.BlogTag.AdminSide;
using Application.Dtos.BlogTag.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.Blogs.BlogTag;
using Domain.Models.Blogs;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BlogTagService : IBlogTagService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<BlogTag> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public BlogTagService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<BlogTag>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> CreateNewBlogTag(AdminSideCreateNewBlogTagViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var tag = new BlogTag()
            {
                Title = model.Title,
            };

            await _dbSet.AddAsync(tag);
            await _context.SaveChangesAsync();

            BlogTagCreatedEvent BlogTagCreatedEvent = new BlogTagCreatedEvent(
                tag.Title);

            await _mediator.PublishEvent(BlogTagCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterBlogTagsViewModel>> FilterBlogTags(AdminSideFilterBlogTagsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterBlogTagsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<BlogTag>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search));
            }

            IQueryable<BlogTag> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<BlogTag>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBlogTagViewModel()
            {
                Id = n.Id,
                Title = n.Title,
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateBlogTagViewModel>> GetBlogTagForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateBlogTagViewModel>(Messages.OperationFailedError);

            var tag = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateBlogTagViewModel()
            {
                Id = n.Id,
                Title = n.Title,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (tag is null) return Result.Failure<AdminSideUpdateBlogTagViewModel>(Messages.OperationFailedError);

            return tag;
        }

        public async Task<Result<List<AdminSideBlogTagAsOptionViewModel>>> GetTagsAsOptions()
        {
            var tags = await _dbSet.Select(n => new AdminSideBlogTagAsOptionViewModel()
            {
                Id = n.Id,
                Title = n.Title
            }).AsNoTracking().ToListAsync();
            if (!tags?.Any() ?? true) return Result.Failure<List<AdminSideBlogTagAsOptionViewModel>>(Messages.OperationFailedError);

            return tags;
        }

        public async Task<Result> SoftDeleteBlogTag(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var tag = await _dbSet.FindAsync(id);
            if (tag is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.Blogs.Any())) return Result.Failure(Messages.CantDeleteThisRecordError);

            tag.IsDeleted = true;

            _dbSet.Update(tag);
            await _context.SaveChangesAsync();

            BlogTagDeletedEvent BlogTagDeletedEvent = new BlogTagDeletedEvent(
                tag.Id);

            await _mediator.PublishEvent(BlogTagDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlogTag(AdminSideUpdateBlogTagViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var tag = await _dbSet.FindAsync(model.Id);
            if (tag is null) return Result.Failure(Messages.OperationFailedError);

            tag.Title = model.Title;

            _dbSet.Update(tag);
            await _context.SaveChangesAsync();

            BlogTagUpdatedEvent BlogTagUpdatedEvent = new BlogTagUpdatedEvent(
            tag.Id,
            tag.Title);

            await _mediator.PublishEvent(BlogTagUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideBlogTagViewModel>>> ClientSideGetBlogTags()
        {
            var tags = await _dbSet.Select(n => new ClientSideBlogTagViewModel()
            {
                Id = n.Id,
                Title = n.Title,
            }).AsNoTracking().ToListAsync();
            if (!tags?.Any() ?? true) return Result.Failure<List<ClientSideBlogTagViewModel>>(Messages.OperationFailedError);

            return tags;
        }
    }
}