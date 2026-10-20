using Application.Dtos.BlogCategory.AdminSide;
using Application.Dtos.BlogCategory.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.Blogs.BlogCategory;
using Domain.Models.Blogs;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BlogCategoryService : IBlogCategoryService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public BlogCategoryService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> CreateNewBlogCategory(AdminSideCreateNewBlogCategoryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _context.BlogCategories
                .AsNoTracking()
                .AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = new BlogCategory()
            {
                Title = model.Title,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                MetaDescription = model.MetaDescription,
                Slug = model.Slug.Replace(" ", "-")
            };

            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();

            BlogCategoryCreatedEvent BlogCategoryCreatedEvent = new BlogCategoryCreatedEvent(
                category.Title,
                category.Slug,
                category.MetaDescription,
                category.Priority,
                category.IsPublished);

            await _mediator.PublishEvent(BlogCategoryCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterBlogCategoriesViewModel>> FilterBlogCategories(AdminSideFilterBlogCategoriesViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterBlogCategoriesViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<BlogCategory>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Slug.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<BlogCategory> query = _context.BlogCategories;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<BlogCategory>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBlogCategoryViewModel()
            {
                Id = n.Id,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Title = n.Title,
                Slug = n.Slug
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateBlogCategoryViewModel>> GetBlogCategoryForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateBlogCategoryViewModel>(Messages.OperationFailedError);

            var category = await _context.BlogCategories
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateBlogCategoryViewModel()
                {
                    Id = n.Id,
                    IsPublished = n.IsPublished,
                    Priority = n.Priority,
                    Title = n.Title,
                    Slug = n.Slug,
                    MetaDescription = n.MetaDescription
                }).FirstOrDefaultAsync();

            if (category is null)
                return Result.Failure<AdminSideUpdateBlogCategoryViewModel>(Messages.OperationFailedError);

            return category;
        }

        public async Task<Result<List<AdminSideBlogCategoryAsOptionViewModel>>> GetCategoriesAsOptions()
        {
            var categories = await _context.BlogCategories
                .AsNoTracking()
                .Where(n => n.IsPublished).OrderBy(n => n.Priority)
                .Select(n => new AdminSideBlogCategoryAsOptionViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug
                }).ToListAsync();

            if (!categories?.Any() ?? true)
                return Result.Failure<List<AdminSideBlogCategoryAsOptionViewModel>>(Messages.OperationFailedError);

            return categories;
        }

        public async Task<Result> SoftDeleteBlogCategory(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var category = await _context.BlogCategories.Where(n => n.Id == id && !n.Blogs.Any()).FirstOrDefaultAsync();

            if (category is null)
                return Result.Failure(Messages.CantDeleteThisRecordError);


            category.IsDeleted = true;

            _context.BlogCategories.Update(category);
            await _context.SaveChangesAsync();

            BlogCategoryDeletedEvent BlogCategoryDeletedEvent = new BlogCategoryDeletedEvent(
                category.Id);

            await _mediator.PublishEvent(BlogCategoryDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlogCategory(AdminSideUpdateBlogCategoryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _context.BlogCategories.
                AsNoTracking()
                .AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-") && n.Id != model.Id))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = await _context.BlogCategories.FindAsync(model.Id);

            if (category is null)
                return Result.Failure(Messages.OperationFailedError);

            category.Title = model.Title;
            category.IsPublished = model.IsPublished;
            category.Priority = model.Priority;
            category.Slug = model.Slug.Replace(" ", "-");
            category.MetaDescription = model.MetaDescription;

            _context.BlogCategories.Update(category);
            await _context.SaveChangesAsync();

            BlogCategoryUpdatedEvent BlogCategoryUpdatedEvent = new BlogCategoryUpdatedEvent(
            category.Id,
            category.Title,
            category.Slug,
            category.MetaDescription,
            category.Priority,
            category.IsPublished);

            await _mediator.PublishEvent(BlogCategoryUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideBlogCategoryViewModel>>> ClientSideGetBlogCategories()
        {
            var categories = await _context.BlogCategories
                .AsNoTracking()
                .Where(n => n.IsPublished).OrderBy(n => n.Priority)
                .Select(n => new ClientSideBlogCategoryViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    MetaDescription = n.MetaDescription
                }).ToListAsync();

            if (!categories?.Any() ?? true)
                return Result.Failure<List<ClientSideBlogCategoryViewModel>>(Messages.OperationFailedError);

            return categories;
        }
    }
}
