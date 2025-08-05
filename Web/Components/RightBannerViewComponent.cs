using Application.Services.Interfaces;
using Application.ViewModels.Banner.ClientSide;
using Domain.Enums.Banner;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class RightBannerViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;
        public RightBannerViewComponent(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _bannerService.ClientSideGetBanners(BannerTypeEnum.Banner560x550);
            if (result.IsFailure) return View("RightBanner", new List<ClientSideBannerViewModel>());

            return View("RightBanner", result.Value);
        }
    }
}
