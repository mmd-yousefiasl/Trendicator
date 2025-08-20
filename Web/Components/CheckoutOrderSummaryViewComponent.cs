using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class CheckoutOrderSummaryViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public CheckoutOrderSummaryViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideCartSummary();
            if (result.IsFailure) return View("CheckoutOrderSummary", new ClientSideCartSummaryViewModel());

            return View("CheckoutOrderSummary", result.Value);
        }
    }
}
