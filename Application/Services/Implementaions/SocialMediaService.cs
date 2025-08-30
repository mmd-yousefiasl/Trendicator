using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Application.ViewModels.SocialMedia.AdminSide;
using Application.ViewModels.SocialMedia.ClientSide;
using Data.Context;
using Domain.Events.SiteInfo.SocialMedia;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SocialMediaService : ISocialMediaService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<SocialMedia> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public SocialMediaService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<SocialMedia>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result> CreateNewSocialMedia(AdminSideCreateNewSocialMediaViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var media = new SocialMedia()
            {
                Url = model.Url,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                Type = model.Type
            };

            await _dbSet.AddAsync(media);
            await _context.SaveChangesAsync();

            SocialMediaCreatedEvent SocialMediaCreatedEvent = new SocialMediaCreatedEvent(
                media.Url,
                media.Priority,
                media.IsPublished,
                media.Type);

            await _mediator.PublishEvent(SocialMediaCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> DeleteSocialMedia(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var media = await _dbSet.FindAsync(id);
            if (media is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(media);
            await _context.SaveChangesAsync();

            SocialMediaDeletedEvent SocialMediaDeletedEvent = new SocialMediaDeletedEvent(
                media.Id);

            await _mediator.PublishEvent(SocialMediaDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterSocialMediaViewModel>> FilterSocialMedia(AdminSideFilterSocialMediaViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterSocialMediaViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SocialMedia>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Url.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            IQueryable<SocialMedia> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<SocialMedia>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideSocialMediaViewModel()
            {
                Id = n.Id,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Type = n.Type,
                Url = n.Url
            }));

            return filter;
        }

        public async Task<Result<List<ClientSideSocialMediaViewModel>>> GetSocialMedia()
        {
            var socialMedia = await _cacheManager.GetAsync(CacheKeys.SocialMediaCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.IsPublished)
                .AsNoTracking().ToListAsync();
            });
            if (!socialMedia?.Any() ?? true) return Result.Failure<List<ClientSideSocialMediaViewModel>>(Messages.OperationFailedError);

            return socialMedia.OrderBy(n => n.Priority)
                .Select(n => new ClientSideSocialMediaViewModel()
                {
                    Type = n.Type,
                    Url = n.Url
                }).ToList();
        }

        public async Task<Result<AdminSideUpdateSocialMediaViewModel>> GetSocialMediaForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateSocialMediaViewModel>(Messages.OperationFailedError);

            var media = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateSocialMediaViewModel()
            {
                Id = n.Id,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Type = n.Type,
                Url = n.Url
            }).AsNoTracking().FirstOrDefaultAsync();
            if (media is null) return Result.Failure<AdminSideUpdateSocialMediaViewModel>(Messages.OperationFailedError);

            return media;
        }

        public async Task<Result> UpdateSocialMedia(AdminSideUpdateSocialMediaViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var media = await _dbSet.FindAsync(model.Id);
            if (media is null) return Result.Failure(Messages.OperationFailedError);

            media.Priority = model.Priority;
            media.IsPublished = model.IsPublished;
            media.Type = model.Type;
            media.Url = model.Url;

            _dbSet.Update(media);
            await _context.SaveChangesAsync();

            SocialMediaUpdatedEvent SocialMediaUpdatedEvent = new SocialMediaUpdatedEvent(
            media.Id,
            media.Url,
            media.Priority,
            media.IsPublished,
            media.Type);

            await _mediator.PublishEvent(SocialMediaUpdatedEvent);

            return Result.Success();
        }
    }
}
