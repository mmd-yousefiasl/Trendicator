using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminFooterViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public AdminFooterViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.GetFooterContent();
            if (result.IsFailure) return View("AdminFooter", "");

            return View("AdminFooter", result.Value);
        }
    }
}
