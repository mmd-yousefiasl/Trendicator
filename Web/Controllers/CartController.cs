using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CartController : BaseController
    {
        #region Fields
        private readonly ICartService _cartService;
        #endregion

        #region Ctor
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        #endregion

        [HttpGet("/cart")]
        public async Task<IActionResult> Cart()
        {
            ViewData["NoIndex"] = true;
            var result = await _cartService.ClientSideGetCart();
            if (result.IsFailure) return View(new List<ClientSideCartDto>());

            return View(result.Value);
        }

        [HttpPost("/cart/add")]
        public async Task<IActionResult> AddToCart([FromBody] ClientSideCartItemToggleDto model)
        {
            if (model?.ProductId < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _cartService.ClientSideAddToCart(model!);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message, inCart = true, cartCount = result.Value });
            }
        }

        [HttpPost("/cart/remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] ClientSideCartItemToggleDto model)
        {
            if (model?.ProductId < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _cartService.ClientSideRemoveFromCart(model!);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message, inCart = false, cartCount = result.Value });
            }
        }

        [HttpPost("/cart/apply-coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ClientApplyCouponDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = Messages.InvalidCouponCodeError });

            var result = await _cartService.ClientSideApplyCoupon(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }

        [HttpPost("/cart/remove-coupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var result = await _cartService.ClientSideRemoveCoupon();
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }

        [Authorize]
        [HttpPost("/cart/checkout"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(bool useWallet = false)
        {
            var result = await _cartService.ClientSideProceedToCheckout(useWallet);
            if (result.IsFailure)
                return Json(new { success = false, message = result.Message });

            if (result.Value.IsDone)
                return Json(new { success = true, url = "/verify-cart" });

            return Json(new { success = true, url = SiteTools.StartPaymentURL + result.Value.TransId });
        }
    }
}