using Application.Dtos.Banner.AdminSide;
using Application.Dtos.Banner.ClientSide;
using Domain.Enums.Banner;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBannerService
    {
        Task<Result<AdminSideFilterBannersViewModel>> AdminSideFilterBanners(AdminSideFilterBannersViewModel filter);

        Task<Result> AdminSideCreateNewBanner(AdminSideCreateNewBannerViewModel model);

        Task<Result<AdminSideUpdateBannerViewModel>> AdminSideGetBannerForUpdate(int id);

        Task<Result> AdminSideUpdateBanner(AdminSideUpdateBannerViewModel model);

        Task<Result> AdminSideDeleteBanner(int id);

        Task<Result<List<ClientSideBannerViewModel>>> ClientSideGetBanners(BannerTypeEnum type);
    }
}
