using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MyOrdersTotalValueViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public MyOrdersTotalValueViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.MyTotalPurchesAmount();
            return View("MyOrdersTotalValue", result);
        }
    }
}
