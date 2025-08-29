using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MySignInDateViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public MySignInDateViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.GetMySignInDate();
            if (result.IsFailure) return View("MySignInDate", DateTime.Now);

            return View("MySignInDate", result.Value);
        }
    }
}
