using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SiteMap.AdminSide;
using Data.Context;
using Domain.Events.SiteInfo.SiteMap;
using Domain.Models.SiteInfo;
using Domain.Shared;
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
        #endregion

        #region Ctor
        public SiteMapService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManer)
        {
            _context = context;
            _dbSet = _context.Set<SiteMap>();
            _mediator = mediator;
            _cacheManager = cacheManer;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewSiteMap(AdminSideCreateNewSiteMapViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var map = new SiteMap()
            {
                Url = model.Url,
                ChagneFreq = model.ChagneFreq,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                Type = model.Type
            };

            await _dbSet.AddAsync(map);
            await _context.SaveChangesAsync();

            SiteMapCreatedEvent SiteMapCreatedEvent = new SiteMapCreatedEvent(
                map.Url,
                map.ChagneFreq,
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


            if (filter.ChagneFreq is not null)
            {
                conditions.Add(n => n.ChagneFreq == filter.ChagneFreq);
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
                ChagneFreq = n.ChagneFreq,
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
                ChagneFreq = n.ChagneFreq,
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
            map.ChagneFreq = model.ChagneFreq;
            map.Priority = model.Priority;
            map.IsPublished = model.IsPublished;
            map.Type = model.Type;
            map.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(map);
            await _context.SaveChangesAsync();

            SiteMapUpdatedEvent SiteMapUpdatedEvent = new SiteMapUpdatedEvent(
            map.Id,
            map.Url,
            map.ChagneFreq,
            map.Priority,
            map.IsPublished,
            map.Type);

            await _mediator.PublishEvent(SiteMapUpdatedEvent);

            return Result.Success();
        }
    }
}
