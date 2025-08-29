using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MyOrdersCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public MyOrdersCountViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.MyOrdersCount();
            return View("MyOrdersCount", result);
        }
    }
}
