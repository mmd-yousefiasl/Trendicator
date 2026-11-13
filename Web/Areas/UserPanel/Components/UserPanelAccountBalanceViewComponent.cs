using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelAccountBalanceViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public UserPanelAccountBalanceViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.GetCurrentUserBalance();

            return View("UserPanelAccountBalance", result);
        }
    }
}
