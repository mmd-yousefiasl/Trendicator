using Application.Dtos.SiteData.AdminSide;
using Application.Dtos.SiteData.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISiteDataService
    {
        Task<Result<AdminSideUpdateSiteDataViewModel>> AdminSideGetSiteDataForUpdate();

        Task<Result> AdminSideUpdateSiteData(AdminSideUpdateSiteDataViewModel model);

        Task<Result<ClientSideSiteDataViewModel>> ClientSideGetSiteData();

        Task<Result<ClientSideSiteLogoViewModel>> ClientSideGetSiteLogo();

        Task<Result<string>> ClientSideGetSitePhone();

        Task<Result<string>> ClientSideGetTermsAndConditions();

        Task<Result<string>> ClientSideGetPrivacyPolicy();

        Task<Result<string>> GetSiteName();

        Task<Result<ClientSideContactUsAddressViewModel>> ClientSideGetContactAddress();

        Task<Result<string>> GetFooterContent();
    }
}
