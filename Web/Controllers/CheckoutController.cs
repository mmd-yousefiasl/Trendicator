using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
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

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
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
            var itemsResult = await _itemService.CheckIfCartItemsIsAvailable();
            if (itemsResult.IsFailure)
            {
                TempData[TemporaryErrorMessage] = itemsResult.Message;
                return RedirectToAction("Cart", "Cart");
            }

            if (!model.UseWallet)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
            }
            else
            {
                TempData[TemporarySuccessMessage] = Messages.SuccessfullyDone;
            }

            return RedirectToAction(nameof(Checkout));
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
    }
}
