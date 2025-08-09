using Application.Services.Interfaces;
using Application.ViewModels.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class StickySiteLogoViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public StickySiteLogoViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteLogo();
            if (result.IsFailure) return View("StickySiteLogo", new ClientSideSiteLogoViewModel());

            return View("StickySiteLogo", result.Value);
        }
    }
}
