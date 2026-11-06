using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserFullNameViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public UserFullNameViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.ClientSideGetCurrentUserFullName();
            if (result.IsFailure)
                return View("UserFullName", string.Empty);

            return View("UserFullName", result.Value);
        }
    }
}
