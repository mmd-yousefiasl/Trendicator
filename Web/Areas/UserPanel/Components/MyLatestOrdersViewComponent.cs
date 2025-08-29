using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MyLatestOrdersViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public MyLatestOrdersViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideFilterCarts(new ClientSideFilterCartsViewModel());
            if (result.IsFailure) return View("MyLatestOrders", new ClientSideFilterCartsViewModel());

            return View("MyLatestOrders", result.Value);
        }
    }
}
