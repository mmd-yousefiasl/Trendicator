using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminAvatarViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public AdminAvatarViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.AdminSideGetAdminAvatar();
            if (result.IsFailure) return View("AdminAvatar", "");

            return View("AdminAvatar", result.Value);
        }
    }
}
