using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Application.ViewModels.Payment;
using Domain.Shared;
using Domain.Statics;
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

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            ViewData["NoIndex"] = true;
            await _cartService.ClientSideAddUserIdToCurrentUserCart();

            var itemsResult = await _itemService.CheckIfCartItemsIsAvailable();
            if (itemsResult.IsFailure)
            {
                TempData[TemporaryErrorMessage] = itemsResult.Message;
                return RedirectToAction("Cart", "Cart");
            }

            return View();
        }

        [HttpPost("/checkout"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(ClientSideCheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.CheckTermsAndConditionsError;
                return RedirectToAction(nameof(Checkout));
            }

            var itemsResult = await _itemService.CheckIfCartItemsIsAvailable();
            if (itemsResult.IsFailure)
            {
                TempData[TemporaryErrorMessage] = itemsResult.Message;
                return RedirectToAction("Cart", "Cart");
            }

            var proceedResult = await _cartService.CheckUserCanProccedToCheckout();
            if (proceedResult.IsFailure)
            {
                TempData[TemporaryErrorMessage] = proceedResult.Message;
                return RedirectToAction(nameof(Checkout));
            }

            var checkoutResult = await _cartService.ClientSideProceedToCheckout(model.UseWallet);
            if (checkoutResult.IsFailure)
            {
                TempData[TemporaryErrorMessage] = checkoutResult.Message;
                return RedirectToAction(nameof(Checkout));
            }

            if (checkoutResult.Value.IsDone)
                return RedirectToAction(nameof(VerifyCart));


            return Redirect(SiteTools.StartPaymentURL + checkoutResult.Value.TransId);
        }

        [HttpPost("/add-coupon"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCoupon(ClientSideAddCouponToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Checkout));
            }
            var result = await _cartService.AddCoupon(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(Checkout));
        }

        [HttpPost("/edit-trading-view-account-name"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTradingViewAccountName(ClientSideUpdateTradingViewAccountNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Checkout));
            }
            var result = await _cartService.ClientSideUpdateCurrentUserTradingViewAccountName(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(Checkout));
        }

        [AllowAnonymous]
        [HttpGet("/payment-result")]
        public async Task<IActionResult> PaymentResult(PaymentResultViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.PaymentFailedError;
                return RedirectToAction(nameof(Checkout));
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
