using Application.Services.Interfaces;
using Application.ViewModels.Cart.AdminSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminLatestOrdersViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public AdminLatestOrdersViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.AdminSideFilterCarts(new AdminSideFilterCartsViewModel());
            if (result.IsFailure) return View("AdminLatestOrders", new AdminSideFilterCartsViewModel());

            return View("AdminLatestOrders", result.Value);
        }
    }
}
