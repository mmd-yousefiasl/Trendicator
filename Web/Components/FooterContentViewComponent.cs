using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterContentViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public FooterContentViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.GetFooterContent();
            if (result.IsFailure) return View("FooterContent", "");

            return View("FooterContent", result.Value);
        }
    }
}
