using Application.Services.Interfaces;
using Application.ViewModels.Banner.ClientSide;
using Domain.Enums.Banner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Org.BouncyCastle.Crypto.Digests;

namespace Web.Components
{
    public class LeftBannerViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;
        public LeftBannerViewComponent(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _bannerService.ClientSideGetBanners(BannerTypeEnum.Banner650x295);
            if (result.IsFailure) return View("LeftBanner", new List<ClientSideBannerViewModel>());

            return View("LeftBanner", result.Value);
        }
    }
}
