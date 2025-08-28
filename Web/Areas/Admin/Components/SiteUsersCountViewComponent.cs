using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class SiteUsersCountViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public SiteUsersCountViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.SiteUsersCount();

            return View("SiteUsersCount", result);
        }
    }
}
