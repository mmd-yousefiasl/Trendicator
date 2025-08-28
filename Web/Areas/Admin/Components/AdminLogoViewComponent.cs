using Application.Services.Interfaces;
using Application.ViewModels.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminLogoViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public AdminLogoViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteLogo();
            if (result.IsFailure) return View("AdminLogo", new ClientSideSiteLogoViewModel());

            return View("AdminLogo", result.Value);
        }
    }
}
