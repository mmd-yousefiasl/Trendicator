using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class SitePhoneViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public SitePhoneViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSitePhone();
            if (result.IsFailure) return View("SitePhone", "");

            return View("SitePhone", result.Value);
        }
    }
}
