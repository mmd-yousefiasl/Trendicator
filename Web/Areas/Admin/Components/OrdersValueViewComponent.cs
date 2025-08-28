using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class OrdersValueViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public OrdersValueViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.OrdersValue();

            return View("OrdersValue", result);
        }
    }
}
