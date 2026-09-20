using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Application.ViewModels.SiteMap.AdminSide;
using Application.ViewModels.SiteMap.ClientSide;
using Data.Context;
using Domain.Enums.SiteInfo;
using Domain.Events.SiteInfo.SiteMap;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SiteMapService : ISiteMapService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<SiteMap> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IBlogCategoryService _blogCategoryService;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        #endregion

        #region Ctor
        public SiteMapService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManer,
            IProductCategoryService productCategoryService,
            IBlogCategoryService blogCategoryService,
            IProductService productService,
            IBlogService blogService)
        {
            _context = context;
            _dbSet = _context.Set<SiteMap>();
            _mediator = mediator;
            _cacheManager = cacheManer;
            _productCategoryService = productCategoryService;
            _blogCategoryService = blogCategoryService;
            _productService = productService;
            _blogService = blogService;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewSiteMap(AdminSideCreateNewSiteMapViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var map = new SiteMap()
            {
                Url = model.Url,
                ChangeFreq = model.ChangeFreq,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                Type = model.Type
            };

            await _dbSet.AddAsync(map);
            await _context.SaveChangesAsync();

            SiteMapCreatedEvent SiteMapCreatedEvent = new SiteMapCreatedEvent(
                map.Url,
                map.ChangeFreq,
                map.Priority,
                map.IsPublished,
                map.Type);

            await _mediator.PublishEvent(SiteMapCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteSiteMap(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var map = await _dbSet.FindAsync(id);
            if (map is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(map);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterSiteMapViewModel>> AdminSideFilterSiteMap(AdminSideFilterSiteMapViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterSiteMapViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SiteMap>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Url.Contains(filter.Search));
            }


            if (filter.ChangeFreq is not null)
            {
                conditions.Add(n => n.ChangeFreq == filter.ChangeFreq);
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            IQueryable<SiteMap> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<SiteMap>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideSiteMapViewModel()
            {
                Id = n.Id,
                ChangeFreq = n.ChangeFreq,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Type = n.Type,
                Url = n.Url
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateSiteMapViewModel>> AdminSideGetSiteMapForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateSiteMapViewModel>(Messages.OperationFailedError);

            var map = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateSiteMapViewModel()
            {
                Id = n.Id,
                Url = n.Url,
                ChangeFreq = n.ChangeFreq,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Type = n.Type,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (map is null)
                return Result.Failure<AdminSideUpdateSiteMapViewModel>(Messages.OperationFailedError);

            return map;
        }

        public async Task<Result> AdminSideUpdateSiteMap(AdminSideUpdateSiteMapViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var map = await _dbSet.FindAsync(model.Id);
            if (map is null) return Result.Failure(Messages.OperationFailedError);

            map.Url = model.Url;
            map.ChangeFreq = model.ChangeFreq;
            map.Priority = model.Priority;
            map.IsPublished = model.IsPublished;
            map.Type = model.Type;
            map.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(map);
            await _context.SaveChangesAsync();

            SiteMapUpdatedEvent SiteMapUpdatedEvent = new SiteMapUpdatedEvent(
            map.Id,
            map.Url,
            map.ChangeFreq,
            map.Priority,
            map.IsPublished,
            map.Type);

            await _mediator.PublishEvent(SiteMapUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateProductCategoriesSiteMap()
        {
            var productCategoriesResult = await _productCategoryService.AdminSideGetCategoriesForSiteMap();
            if (productCategoriesResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var categories = productCategoriesResult.Value;

            // Transaction ensures atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            var existingMaps = await _dbSet
                .Where(n => n.Type == SiteMapTypeEnum.ProductCategory)
                .ToListAsync();

            if (existingMaps.Any())
                _dbSet.RemoveRange(existingMaps);

            var newMaps = categories.Select(n => new SiteMap
            {
                ChangeFreq = ChangeFreqEnum.Weekly,
                Url = $"{SiteTools.SiteUrl}products/{n.Slug}",
                IsPublished = true,
                Type = SiteMapTypeEnum.ProductCategory,
                Priority = 0.7m
            }).ToList();

            await _dbSet.AddRangeAsync(newMaps);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteAllLinks()
        {
            var maps = await _dbSet.ToListAsync();
            if (!maps.Any()) return Result.Failure(Messages.OperationFailedError);

            _dbSet.RemoveRange(maps);
            await _context.SaveChangesAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteAllExceptOtherPages()
        {
            var maps = await _dbSet.Where(n => n.Type != SiteMapTypeEnum.OtherPages).ToListAsync();
            if (!maps.Any()) return Result.Failure(Messages.OperationFailedError);

            _dbSet.RemoveRange(maps);
            await _context.SaveChangesAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlogCategoriesSiteMap()
        {
            var blogCategoriesResult = await _blogCategoryService.GetCategoriesAsOptions();
            if (blogCategoriesResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var categories = blogCategoriesResult.Value;

            // Transaction ensures atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            var existingMaps = await _dbSet
                .Where(n => n.Type == SiteMapTypeEnum.BlogCategory)
                .ToListAsync();

            if (existingMaps.Any())
                _dbSet.RemoveRange(existingMaps);

            var newMaps = categories.Select(n => new SiteMap
            {
                ChangeFreq = ChangeFreqEnum.Weekly,
                Url = $"{SiteTools.SiteUrl}blogs/{n.Slug}",
                IsPublished = true,
                Type = SiteMapTypeEnum.BlogCategory,
                Priority = 0.7m
            }).ToList();

            await _dbSet.AddRangeAsync(newMaps);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateProductsSiteMap()
        {
            var productResult = await _productService.AdminSideGetProductIdsForSiteMap();
            if (productResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var Ids = productResult.Value;

            // Transaction ensures atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            var existingMaps = await _dbSet
                .Where(n => n.Type == SiteMapTypeEnum.Product)
                .ToListAsync();

            if (existingMaps.Any())
                _dbSet.RemoveRange(existingMaps);

            var newMaps = Ids.Select(n => new SiteMap
            {
                ChangeFreq = ChangeFreqEnum.Weekly,
                Url = $"{SiteTools.SiteUrl}p/{n.Id}/{n.Slug}",
                IsPublished = true,
                Type = SiteMapTypeEnum.Product,
                Priority = 0.9m
            }).ToList();

            await _dbSet.AddRangeAsync(newMaps);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateBlogsSiteMap()
        {
            var blogResult = await _blogService.AdminSideGetBlogIdsForSiteMap();
            if (blogResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var Ids = blogResult.Value;

            // Transaction ensures atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            var existingMaps = await _dbSet
                .Where(n => n.Type == SiteMapTypeEnum.Blog)
                .ToListAsync();

            if (existingMaps.Any())
                _dbSet.RemoveRange(existingMaps);

            var newMaps = Ids.Select(n => new SiteMap
            {
                ChangeFreq = ChangeFreqEnum.Monthly,
                Url = $"{SiteTools.SiteUrl}b/{n.Id}/{n.Slug}",
                IsPublished = true,
                Type = SiteMapTypeEnum.Blog,
                Priority = 0.9m
            }).ToList();

            await _dbSet.AddRangeAsync(newMaps);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SiteMapBulkUpdatedEvent SiteMapBulkUpdatedEvent = new SiteMapBulkUpdatedEvent(true);

            await _mediator.PublishEvent(SiteMapBulkUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateAllSiteMap()
        {
            var pCategoryResult = await UpdateProductCategoriesSiteMap();
            if (pCategoryResult.IsFailure) return Result.Failure("خطایی هنگام بروزرسانی دسته بندی محصولات رخ داد");

            var bCategoryResult = await UpdateBlogCategoriesSiteMap();
            if (bCategoryResult.IsFailure) return Result.Failure("خطایی هنگام بروزرسانی دسته بندی مقالات رخ داد");

            var productResult = await UpdateProductsSiteMap();
            if (productResult.IsFailure) return Result.Failure("خطایی هنگام بروزرسانی محصولات رخ داد");

            var blogResult = await UpdateBlogsSiteMap();
            if (blogResult.IsFailure) return Result.Failure("خطایی هنگام بروزرسانی مقالات رخ داد");

            return Result.Success();
        }

        public async Task<Result<List<ClientSideSiteMapViewModel>>> ClientSideGetSiteMap()
        {
            var maps = await _cacheManager.GetAsync(CacheKeys.SiteMapCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.IsPublished).OrderByDescending(n => n.Priority)
                .Select(n => new ClientSideSiteMapViewModel()
                {
                    Url = n.Url,
                    LastUpdateDate = n.LastUpdatedDate,
                    ChangeFreq = n.ChangeFreq,
                    Priority = n.Priority
                }).AsNoTracking().ToListAsync();
            });

            if (!maps?.Any() ?? true)
                return Result.Failure<List<ClientSideSiteMapViewModel>>(Messages.OperationFailedError);

            return maps;
        }
    }
}
