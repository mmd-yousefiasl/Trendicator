using Application.Services.Interfaces;
using Application.Dtos.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class SiteLogoViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public SiteLogoViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteLogo();
            if (result.IsFailure) return View("SiteLogo", new ClientSideSiteLogoViewModel());

            return View("SiteLogo", result.Value);
        }
    }
}
