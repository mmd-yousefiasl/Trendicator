using Application.Services.Interfaces;
using Application.ViewModels.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelLogoViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public UserPanelLogoViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteLogo();
            if (result.IsFailure) return View("UserPanelLogo", new ClientSideSiteLogoViewModel());

            return View("UserPanelLogo", result.Value);
        }
    }
}
