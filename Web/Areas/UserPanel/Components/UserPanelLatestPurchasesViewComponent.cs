using Application.Dtos.Cart.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelLatestPurchasesViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public UserPanelLatestPurchasesViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideFilterCarts(new ClientSideFilterCartsDto() { TakeEntity = 5 });

            if (result.IsFailure)
                return View("UserPanelLatestPurchases", new ClientSideFilterCartsDto());

            return View("UserPanelLatestPurchases", result.Value);
        }
    }
}
