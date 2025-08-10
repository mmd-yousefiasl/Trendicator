using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SiteData.AdminSide;
using Application.ViewModels.SiteData.ClientSide;
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
        #endregion

        #region Ctor
        public SiteDataService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<SiteData>();
            _mediator = mediator;
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

                model.DarkBgLogoFile.UplaodFileToServer(fileName, SiteTools.SiteLogo);
                siteData.DarkBgLogo = fileName;
            }

            if (model.LightBgLogoFile is not null)
            {
                if (!model.LightBgLogoFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.LightBgLogoFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LightBgLogoFile.FileName);
                siteData.LightBgLogo?.RemoveFileFromServer(SiteTools.SiteLogo);

                model.LightBgLogoFile.UplaodFileToServer(fileName, SiteTools.SiteLogo);

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
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => new ClientSideSiteDataViewModel()
            {
                SiteName = n.SiteName,
                Address = n.Address,
                DarkBgLogo = n.DarkBgLogo,
                LightBgLogo = n.LightBgLogo,
                Email = n.Email,
                FooterDescription = n.FooterDescription,
                Phone = n.Phone,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (data is null) return Result.Failure<ClientSideSiteDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<ClientSideSiteLogoViewModel>> ClientSideGetSiteLogo()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => new ClientSideSiteLogoViewModel()
            {
                SiteName = n.SiteName,
                DarkBgLogo = n.DarkBgLogo,
                LightBgLogo = n.LightBgLogo
            }).AsNoTracking().FirstOrDefaultAsync();
            if (data is null) return Result.Failure<ClientSideSiteLogoViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<string>> ClientSideGetSitePhone()
        {
            var phone = await _dbSet.Where(n => n.Id == 1).Select(n => n.Phone).AsNoTracking().FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(phone)) return Result.Failure<string>(Messages.OperationFailedError);

            return phone;
        }

        public async Task<Result<string>> ClientSideGetTermsAndConditions()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => n.TermsAndCondition).AsNoTracking().FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(data)) return Result.Failure<string>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<string>> ClientSideGetPrivacyPolicy()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => n.PrivacyPolicy).AsNoTracking().FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(data)) return Result.Failure<string>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<string>> GetSiteName()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => n.SiteName).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(data)) return Result.Failure<string>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<ClientSideContactUsAddressViewModel>> ClientSideGetContactAddress()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => new ClientSideContactUsAddressViewModel()
            {
                Address = n.Address,
                Email = n.Email,
                Phone = n.Phone
            }).AsNoTracking().FirstOrDefaultAsync();
            if (data is null) return Result.Failure<ClientSideContactUsAddressViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<string>> GetFooterContent()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => n.FooterContent).AsNoTracking().FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(data)) return Result.Failure<string>(Messages.OperationFailedError);

            return data;
        }
    }
}
