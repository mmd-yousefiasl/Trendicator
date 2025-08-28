using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class OrdersCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public OrdersCountViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.OrdersCount();
            return View("OrdersCount", result);
        }
    }
}
