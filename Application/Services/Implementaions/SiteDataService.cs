using Application.Dtos.SiteData.AdminSide;
using Application.Dtos.SiteData.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SiteDataService : ISiteDataService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<SiteData> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public SiteDataService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<SiteData>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideUpdateSiteDataViewModel>> AdminSideGetSiteDataForUpdate()
        {
            var siteData = await _dbSet.Where(n => n.Id == 1).Select(n => new AdminSideUpdateSiteDataViewModel()
            {
                DarkBgLogoFileName = n.DarkBgLogo,
                LightBgLogoFileName = n.LightBgLogo,
                Address = n.Address,
                Email = n.Email,
                FooterDescription = n.FooterDescription,
                FooterContent = n.FooterContent,
                Phone = n.Phone,
                PrivacyPolicy = n.PrivacyPolicy,
                TermsAndCondition = n.TermsAndCondition,
                SiteName = n.SiteName,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (siteData is null) return Result.Failure<AdminSideUpdateSiteDataViewModel>(Messages.OperationFailedError);

            return siteData;
        }

        public async Task<Result> AdminSideUpdateSiteData(AdminSideUpdateSiteDataViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var siteData = await _dbSet.Where(n => n.Id == 1).FirstOrDefaultAsync();
            if (siteData is null) return Result.Failure(Messages.OperationFailedError);

            if (model.DarkBgLogoFile is not null)
            {
                if (!model.DarkBgLogoFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.DarkBgLogoFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.DarkBgLogoFile.FileName);
                siteData.DarkBgLogo?.RemoveFileFromServer(SiteTools.SiteLogo);

                model.DarkBgLogoFile.UploadFileToServer(fileName, SiteTools.SiteLogo);
                siteData.DarkBgLogo = fileName;
            }

            if (model.LightBgLogoFile is not null)
            {
                if (!model.LightBgLogoFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.LightBgLogoFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LightBgLogoFile.FileName);
                siteData.LightBgLogo?.RemoveFileFromServer(SiteTools.SiteLogo);

                model.LightBgLogoFile.UploadFileToServer(fileName, SiteTools.SiteLogo);

                siteData.LightBgLogo = fileName;
            }

            siteData.SiteName = model.SiteName;
            siteData.Phone = model.Phone;
            siteData.Email = model.Email;
            siteData.FooterDescription = model.FooterDescription;
            siteData.Address = model.Address;
            siteData.TermsAndCondition = model.TermsAndCondition;
            siteData.PrivacyPolicy = model.PrivacyPolicy;
            siteData.FooterContent = model.FooterContent;

            _dbSet.Update(siteData);
            await _context.SaveChangesAsync();

            SiteDataUpdatedEvent SiteDataUpdatedEvent = new SiteDataUpdatedEvent(
                siteData.DarkBgLogo,
                siteData.LightBgLogo,
                siteData.Phone,
                siteData.FooterDescription,
                siteData.Address,
                siteData.Email,
                siteData.TermsAndCondition,
                siteData.PrivacyPolicy);

            await _mediator.PublishEvent(SiteDataUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<ClientSideSiteDataViewModel>> ClientSideGetSiteData()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null) return Result.Failure<ClientSideSiteDataViewModel>(Messages.OperationFailedError);

            return new ClientSideSiteDataViewModel()
            {
                Address = data.Address,
                DarkBgLogo = data.DarkBgLogo,
                SiteName = data.SiteName,
                Email = data.Email,
                FooterDescription = data.FooterDescription,
                LightBgLogo = data.LightBgLogo,
                Phone = data.Phone
            };
        }

        public async Task<Result<ClientSideSiteLogoViewModel>> ClientSideGetSiteLogo()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null) return Result.Failure<ClientSideSiteLogoViewModel>(Messages.OperationFailedError);

            return new ClientSideSiteLogoViewModel()
            {
                DarkBgLogo = data.DarkBgLogo,
                LightBgLogo = data.LightBgLogo,
                SiteName = data.SiteName,
            };
        }

        public async Task<Result<string>> ClientSideGetSitePhone()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null || string.IsNullOrWhiteSpace(data.Phone)) return Result.Failure<string>(Messages.OperationFailedError);

            return data.Phone;
        }

        public async Task<Result<string>> ClientSideGetTermsAndConditions()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null || string.IsNullOrWhiteSpace(data.TermsAndCondition)) return Result.Failure<string>(Messages.OperationFailedError);

            return data.TermsAndCondition;
        }

        public async Task<Result<string>> ClientSideGetPrivacyPolicy()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null || string.IsNullOrWhiteSpace(data.PrivacyPolicy)) return Result.Failure<string>(Messages.OperationFailedError);

            return data.PrivacyPolicy;
        }

        public async Task<Result<string>> GetSiteName()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null || string.IsNullOrWhiteSpace(data.SiteName)) return Result.Failure<string>(Messages.OperationFailedError);

            return data.SiteName;
        }

        public async Task<Result<ClientSideContactUsAddressViewModel>> ClientSideGetContactAddress()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null) return Result.Failure<ClientSideContactUsAddressViewModel>(Messages.OperationFailedError);

            return new ClientSideContactUsAddressViewModel()
            {
                Address = data.Address,
                Email = data.Email,
                Phone = data.Phone
            };
        }

        public async Task<Result<string>> GetFooterContent()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SiteDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null || string.IsNullOrWhiteSpace(data.FooterContent)) return Result.Failure<string>(Messages.OperationFailedError);

            return data.FooterContent;
        }
    }
}
