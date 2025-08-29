using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelFooterViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public UserPanelFooterViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.GetFooterContent();
            if (result.IsFailure) return View("UserPanelFooter", "");

            return View("UserPanelFooter", result.Value);
        }
    }
}
