using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class CartItemsCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public CartItemsCountViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideGetCartItemsCount();
            return View("CartItemsCount", result);
        }
    }
}
