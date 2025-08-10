using Application.Services.Interfaces;
using Application.ViewModels.Banner.ClientSide;
using Domain.Enums.Banner;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterPaymentMethodsViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;
        public FooterPaymentMethodsViewComponent(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _bannerService.ClientSideGetBanners(BannerTypeEnum.PaymentMethods);
            if (result.IsFailure) return View("FooterPaymentMethods", new List<ClientSideBannerViewModel>());

            return View("FooterPaymentMethods", result.Value);
        }
    }
}
