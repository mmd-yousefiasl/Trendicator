using Application.ViewModels.SiteMap.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISiteMapService
    {
        Task<Result<AdminSideFilterSiteMapViewModel>> AdminSideFilterSiteMap(AdminSideFilterSiteMapViewModel filter);

        Task<Result> AdminSideCreateNewSiteMap(AdminSideCreateNewSiteMapViewModel model);

        Task<Result<AdminSideUpdateSiteMapViewModel>> AdminSideGetSiteMapForUpdate(int id);

        Task<Result> AdminSideUpdateSiteMap(AdminSideUpdateSiteMapViewModel model);

        Task<Result> AdminSideDeleteSiteMap(int id);
    }
}
