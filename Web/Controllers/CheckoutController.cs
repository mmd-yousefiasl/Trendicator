using Application.Dtos.Payment;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class CheckoutController : BaseController
    {
        #region Fields
        private readonly ICartService _cartService;
        private readonly ICartItemService _itemService;
        #endregion

        #region Ctor
        public CheckoutController(ICartService cartService,
            ICartItemService itemService)
        {
            _cartService = cartService;
            _itemService = itemService;
        }
        #endregion

        [AllowAnonymous]
        [HttpGet("/payment-result")]
        public async Task<IActionResult> PaymentResult(PaymentResultViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.PaymentFailedError;
                return RedirectToAction("Cart", "Cart", new { area = "" });
            }
            var result = await _cartService.ClientSideCheckout(model);
            if (result.IsFailure)
            {
                return RedirectToAction(nameof(PaymentFailed));
            }

            return RedirectToAction(nameof(VerifyCart));
        }

        [HttpGet("/verify-cart")]
        public async Task<IActionResult> VerifyCart()
        {
            var result = await _cartService.ClientSideVerifyCart();
            if (result.IsFailure) return RedirectToAction(nameof(PaymentFailed));

            return RedirectToAction(nameof(PaymentSuccess), new { orderId = result.Value });
        }

        [HttpGet("/payment/success/{orderId}")]
        public IActionResult PaymentSuccess(int orderId)
        {
            ViewData["NoIndex"] = true;
            return View(orderId);
        }

        [HttpGet("/payment/failed")]
        public IActionResult PaymentFailed()
        {
            ViewData["NoIndex"] = true;
            return View();
        }
    }
}
