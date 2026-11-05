using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class UseUserWalletViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UseUserWalletViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userService.GetCurrentUserBalance();

            return View("UseUserWallet", result);
        }
    }
}