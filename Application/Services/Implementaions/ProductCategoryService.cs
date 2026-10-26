using Application.Dtos.ProductCategory.AdminSide;
using Application.Dtos.ProductCategory.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Data.Context;
using Domain.Events.Products.ProductCategory;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductCategoryService : IProductCategoryService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public ProductCategoryService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewProductCategory(AdminSideCreateNewProductCategoryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _context.ProductCategories.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = new ProductCategory()
            {
                Title = model.Title,
                ParentCategoryId = model.ParentCategoryId,
                Slug = model.Slug.Replace(" ", "-"),
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                MetaDescription = model.MetaDescription
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            ProductCategoryCreatedEvent ProductCategoryCreatedEvent = new ProductCategoryCreatedEvent(
                category.Title,
                category.Slug,
                category.MetaDescription,
                category.Priority,
                category.IsPublished,
                category.ParentCategoryId);

            await _mediator.PublishEvent(ProductCategoryCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductCategory(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var category = await _context.ProductCategories
                .Where(n => n.Id == id && !n.Products.Any() && !n.SubCategories.Any())
                .FirstOrDefaultAsync();
            if (category is null) return Result.Failure(Messages.CantDeleteThisRecordError);

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            ProductCategoryDeletedEvent ProductCategoryDeletedEvent = new ProductCategoryDeletedEvent(
                category.Id);

            await _mediator.PublishEvent(ProductCategoryDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductCategoriesViewMdoel>> AdminSideFilterProductCategories(AdminSideFilterProductCategoriesViewMdoel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductCategoriesViewMdoel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductCategory>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.ParentCategory.Title.Contains(filter.Search) ||
                n.Slug.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<ProductCategory> query = _context.ProductCategories;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductCategory>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductCategoryViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                ParentCategoryTitle = n.ParentCategory.Title,
                IsPublished = n.IsPublished,
                Priority = n.Priority
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductCategoryViewModel>> AdminSideGetCategoryForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateProductCategoryViewModel>(Messages.OperationFailedError);

            var category = await _context.ProductCategories
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateProductCategoryViewModel()
                {
                    Id = n.Id,
                    ParentCategoryId = n.ParentCategoryId,
                    Title = n.Title,
                    Slug = n.Slug,
                    IsPublished = n.IsPublished,
                    MetaDescription = n.MetaDescription,
                    Priority = n.Priority
                }).FirstOrDefaultAsync();

            if (category is null)
                return Result.Failure<AdminSideUpdateProductCategoryViewModel>(Messages.OperationFailedError);

            return category;
        }

        public async Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetProductCategoriesAsOptions(int? id)
        {
            IQueryable<ProductCategory> query = _context.ProductCategories;
            if (id > 0) query = query.Where(n => n.Id != id);

            query = query.Where(n => n.IsPublished);
            query = query.OrderBy(n => n.Priority);

            var categories = await query
                .AsNoTracking()
                .Select(n => new AdminSideProductCategoryAsOptionViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug
                }).ToListAsync();

            if (!categories.Any())
                return Result.Failure<List<AdminSideProductCategoryAsOptionViewModel>>(Messages.OperationFailedError);

            return categories;
        }

        public async Task<Result> AdminSideUpdateProductCategory(AdminSideUpdateProductCategoryViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            if (await _context.ProductCategories.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-") && n.Id != model.Id))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = await _context.ProductCategories.FindAsync(model.Id);

            if (category is null)
                return Result.Failure(Messages.OperationFailedError);

            category.Title = model.Title;
            category.ParentCategoryId = model.ParentCategoryId;
            category.Slug = model.Slug.Replace(" ", "-");
            category.IsPublished = model.IsPublished;
            category.Priority = model.Priority;
            category.MetaDescription = model.MetaDescription;

            _context.ProductCategories.Update(category);
            await _context.SaveChangesAsync();

            ProductCategoryUpdatedEvent ProductCategoryUpdatedEvent = new ProductCategoryUpdatedEvent(
                category.Id,
                category.Title,
                category.Slug,
                category.MetaDescription,
                category.Priority,
                category.IsPublished,
                category.ParentCategoryId);

            await _mediator.PublishEvent(ProductCategoryUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetProductCategories()
        {
            var categories = await _cacheManager.GetAsync(CacheKeys.ProductCategoriesCacheKey, async () =>
            {
                return await _context.ProductCategories
                .AsNoTracking()
                .Where(n => n.IsPublished)
                .OrderBy(n => n.Priority)
                .ToListAsync();
            });

            if (!categories?.Any() ?? true)
                return Result.Failure<List<ClientSideProductCategoryViewModel>>(Messages.OperationFailedError);

            var firstParents = new List<ClientSideProductCategoryViewModel>();
            foreach (var item in categories.Where(n => n.ParentCategoryId == null).ToList() ?? new List<ProductCategory>())
            {
                ClientSideProductCategoryViewModel firstParent = new ClientSideProductCategoryViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Slug = item.Slug,
                    ParentId = null,
                    Categories = new List<ClientSideProductCategoryViewModel>()
                };
                foreach (var item2 in categories.Where(n => n.ParentCategoryId == item.Id).ToList() ?? new List<ProductCategory>())
                {
                    ClientSideProductCategoryViewModel secondParent = new ClientSideProductCategoryViewModel()
                    {
                        Id = item2.Id,
                        Title = item2.Title,
                        Slug = item2.Slug,
                        ParentId = item.Id,
                        Categories = new List<ClientSideProductCategoryViewModel>()
                    };
                    foreach (var item3 in categories.Where(n => n.ParentCategoryId == item2.Id).ToList() ?? new List<ProductCategory>())
                    {
                        ClientSideProductCategoryViewModel sub = new ClientSideProductCategoryViewModel()
                        {
                            Id = item3.Id,
                            Title = item3.Title,
                            Slug = item3.Slug,
                            ParentId = item2.Id,
                        };

                        secondParent.Categories.Add(sub);
                    }

                    firstParent.Categories.Add(secondParent);
                }

                firstParents.Add(firstParent);
            }
            if (!firstParents.Any()) return Result.Failure<List<ClientSideProductCategoryViewModel>>(Messages.OperationFailedError);

            return firstParents;
        }

        public async Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetCategoriesForSiteMap()
        {
            var categories = await _context.ProductCategories
                .AsNoTracking()
                .Select(n => new AdminSideProductCategoryAsOptionViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug
                }).ToListAsync();

            if (!categories.Any())
                return Result.Failure<List<AdminSideProductCategoryAsOptionViewModel>>(Messages.OperationFailedError);

            return categories;
        }

        public async Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetParentCategories()
        {
            var categories = await _context.ProductCategories
                .AsNoTracking()
                .Where(n => n.IsPublished && n.ParentCategoryId == null)
                .OrderBy(n => n.Priority)
                .ToListAsync();

            if (!categories?.Any() ?? true)
                return Result.Failure<List<ClientSideProductCategoryViewModel>>(Messages.OperationFailedError);

            return categories.Select(n => new ClientSideProductCategoryViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
            }).ToList();
        }
    }
}
