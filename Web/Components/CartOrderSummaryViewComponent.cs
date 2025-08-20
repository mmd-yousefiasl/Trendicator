using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class CartOrderSummaryViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public CartOrderSummaryViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideCartSummary();
            if (result.IsFailure) return View("CartOrderSummary", new ClientSideCartSummaryViewModel());

            return View("CartOrderSummary", result.Value);
        }
    }
}