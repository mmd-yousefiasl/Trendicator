using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Application.ViewModels.ProductCategory.AdminSide;
using Application.ViewModels.ProductCategory.ClientSide;
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
        private protected DbSet<ProductCategory> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public ProductCategoryService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<ProductCategory>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewProductCategory(AdminSideCreateNewProductCategoryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _dbSet.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = new ProductCategory()
            {
                Title = model.Title,
                ParentCategoryId = model.ParentCategoryId,
                Slug = model.Slug.Replace(" ", "-")
            };

            await _dbSet.AddAsync(category);
            await _context.SaveChangesAsync();

            ProductCategoryCreatedEvent ProductCategoryCreatedEvent = new ProductCategoryCreatedEvent(
                category.Title,
                category.Slug);

            await _mediator.PublishEvent(ProductCategoryCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductCategory(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.Products.Any())) return Result.Failure(Messages.CantDeleteThisRecordError);

            var category = await _dbSet.FindAsync(id);
            if (category is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(category);
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

            IQueryable<ProductCategory> query = _dbSet;
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
                ParentCategoryTitle = n.ParentCategory.Title
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductCategoryViewModel>> AdminSideGetCategoryForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateProductCategoryViewModel>(Messages.OperationFailedError);

            var category = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateProductCategoryViewModel()
            {
                Id = n.Id,
                ParentCategoryId = n.ParentCategoryId,
                Title = n.Title,
                Slug = n.Slug,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (category is null) return Result.Failure<AdminSideUpdateProductCategoryViewModel>(Messages.OperationFailedError);

            return category;
        }

        public async Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetProductCategoriesAsOptions(int? id)
        {
            IQueryable<ProductCategory> query = _dbSet;
            if (id > 0) query = query.Where(n => n.Id != id);

            var categories = await query.Select(n => new AdminSideProductCategoryAsOptionViewModel()
            {
                Id = n.Id,
                Title = n.Title
            }).AsNoTracking().ToListAsync();
            if (!categories.Any()) return Result.Failure<List<AdminSideProductCategoryAsOptionViewModel>>(Messages.OperationFailedError);

            return categories;
        }

        public async Task<Result> AdminSideUpdateProductCategory(AdminSideUpdateProductCategoryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _dbSet.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-") && n.Id != model.Id))
                return Result.Failure(Messages.DuplicateSlugError);

            var category = await _dbSet.FindAsync(model.Id);
            if (category is null) return Result.Failure(Messages.OperationFailedError);

            category.Title = model.Title;
            category.ParentCategoryId = model.ParentCategoryId;
            category.Slug = model.Slug.Replace(" ", "-");

            _dbSet.Update(category);
            await _context.SaveChangesAsync();

            ProductCategoryUpdatedEvent ProductCategoryUpdatedEvent = new ProductCategoryUpdatedEvent(
                category.Id,
                category.Title,
                category.Slug);

            await _mediator.PublishEvent(ProductCategoryUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetProductCategories()
        {
            var categories = await _cacheManager.GetAsync(CacheKeys.ProductCategoriesCacheKey, async () =>
            {
                return await _dbSet.AsNoTracking().ToListAsync();
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
    }
}
