using Application.ViewModels.SiteMap.AdminSide;
using Application.ViewModels.SiteMap.ClientSide;
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

        Task<Result> UpdateProductCategoriesSiteMap();

        Task<Result> AdminSideDeleteAllLinks();

        Task<Result> AdminSideDeleteAllExceptOtherPages();

        Task<Result> UpdateBlogCategoriesSiteMap();

        Task<Result> UpdateProductsSiteMap();

        Task<Result> UpdateBlogsSiteMap();

        Task<Result> UpdateAllSiteMap();

        Task<Result<List<ClientSideSiteMapViewModel>>> ClientSideGetSiteMap();
    }
}
