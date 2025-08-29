using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelAvatarViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public UserPanelAvatarViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.AdminSideGetAdminAvatar();
            if (result.IsFailure) return View("UserPanelAvatar", "");

            return View("UserPanelAvatar", result.Value);
        }
    }
}
