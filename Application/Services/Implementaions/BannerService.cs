using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Application.ViewModels.Banner.AdminSide;
using Application.ViewModels.Banner.ClientSide;
using Data.Context;
using Domain.Enums.Banner;
using Domain.Events.SiteInfo.Banner;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class BannerService : IBannerService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Banner> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public BannerService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<Banner>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideFilterBannersViewModel>> AdminSideFilterBanners(AdminSideFilterBannersViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterBannersViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Banner>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Url.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            IQueryable<Banner> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Banner>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBannerViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Type = n.Type,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewBanner(AdminSideCreateNewBannerViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);

            model.File.UploadFileToServer(fileName, SiteTools.Banner);

            Banner banner = new Banner()
            {
                FileName = fileName,
                Title = model.Title,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                Type = model.Type,
                Url = model.Url
            };

            await _dbSet.AddAsync(banner);
            await _context.SaveChangesAsync();

            BannerCreatedEvent BannerCreatedEvent = new BannerCreatedEvent(
                banner.FileName,
                banner.Title,
                banner.Url,
                banner.Priority,
                banner.IsPublished);

            await _mediator.PublishEvent(BannerCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateBannerViewModel>> AdminSideGetBannerForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateBannerViewModel>(Messages.OperationFailedError);

            var banner = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateBannerViewModel()
            {
                Id = n.Id,
                FileName = n.FileName,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Title = n.Title,
                Type = n.Type,
                Url = n.Url
            }).AsNoTracking().FirstOrDefaultAsync();
            if (banner is null) return Result.Failure<AdminSideUpdateBannerViewModel>(Messages.OperationFailedError);

            return banner;
        }

        public async Task<Result> AdminSideUpdateBanner(AdminSideUpdateBannerViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var banner = await _dbSet.FindAsync(model.Id);
            if (banner is null) return Result.Failure(Messages.OperationFailedError);

            if (model.File is not null)
            {
                if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);

                model.File.UploadFileToServer(fileName, SiteTools.Banner);
                banner.FileName.RemoveFileFromServer(SiteTools.Banner);

                banner.FileName = fileName;
            }

            banner.Title = model.Title;
            banner.Url = model.Url;
            banner.IsPublished = model.IsPublished;
            banner.Priority = model.Priority;
            banner.Type = model.Type;

            _dbSet.Update(banner);
            await _context.SaveChangesAsync();

            BannerUpdatedEvent BannerUpdatedEvent = new BannerUpdatedEvent(
            banner.Id,
            banner.FileName,
            banner.Title,
            banner.Url,
            banner.Priority,
            banner.IsPublished);

            await _mediator.PublishEvent(BannerUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteBanner(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var banner = await _dbSet.FindAsync(id);
            if (banner is null) return Result.Failure(Messages.OperationFailedError);

            banner.FileName.RemoveFileFromServer(SiteTools.Banner);

            _dbSet.Remove(banner);
            await _context.SaveChangesAsync();

            BannerDeletedEvent BannerDeletedEvent = new BannerDeletedEvent(
                banner.Id);

            await _mediator.PublishEvent(BannerDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideBannerViewModel>>> ClientSideGetBanners(BannerTypeEnum type)
        {
            var banners = await _cacheManager.GetAsync(CacheKeys.BannerCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.IsPublished)
                .AsNoTracking().ToListAsync();
            });
            if (!banners?.Any() ?? true) return Result.Failure<List<ClientSideBannerViewModel>>(Messages.OperationFailedError);

            return banners.OrderBy(n => n.Priority).Where(n => n.Type == type).Select(n => new ClientSideBannerViewModel()
            {
                Title = n.Title,
                FileName = n.FileName,
                Url = n.Url
            }).ToList();
        }
    }
}
