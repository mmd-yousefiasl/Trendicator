using Application.Services.Interfaces;
using Application.Dtos.Cart.ClientSide;
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
            if (result.IsFailure)
                return View("UseUserWallet", new ClientSideCheckoutViewModel());


            return View("UseUserWallet", new ClientSideCheckoutViewModel()
            {
                Balance = result.Value,
                UseWallet = false
            });
        }
    }
}