using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class SiteNameViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public SiteNameViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.GetSiteName();
            if (result.IsFailure) return View("SiteName", "");

            return View("SiteName", result.Value);
        }
    }
}
