using Application.Services.Interfaces;
using Application.Dtos.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MobileSiteLogoViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public MobileSiteLogoViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteLogo();
            if (result.IsFailure) return View("MobileSiteLogo", new ClientSideSiteLogoViewModel());

            return View("MobileSiteLogo", result.Value);
        }
    }
}
