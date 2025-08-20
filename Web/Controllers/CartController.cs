using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Web.Controllers
{
    public class CartController : BaseController
    {
        #region Fields
        private readonly ICartService _cartService;
        private readonly ICartItemService _itemService;
        #endregion

        #region Ctor
        public CartController(ICartService cartService,
            ICartItemService itemService)
        {
            _cartService = cartService;
            _itemService = itemService;
        }
        #endregion

        [HttpGet("/add-to-cart/{optionId}")]
        public async Task<IActionResult> AddToCart(int optionId)
        {
            if (optionId < 1) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _cartService.AddToCart(optionId);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        [HttpGet("/cart-modal")]
        public async Task<IActionResult> CartModal()
        {
            var result = await _cartService.ClientSideGetCart();
            if (result.IsFailure) return PartialView("_CartModal", new List<ClientSideCartViewModel>());

            return PartialView("_CartModal", result.Value);
        }

        [HttpGet("/remove-cart-modal-item/{id}")]
        public async Task<IActionResult> RemoveCartModalItem(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _itemService.ClientSideDeleteItemFromCart(id);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        [HttpGet("/cart")]
        public async Task<IActionResult> Cart()
        {
            var result = await _cartService.ClientSideGetCart();
            if (result.IsFailure) return View(new List<ClientSideCartViewModel>());

            return View(result.Value);
        }

        [HttpGet("/cart/remove/{id}")]
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Cart));
            }
            var result = await _itemService.ClientSideDeleteItemFromCart(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(Cart));
        }
    }
}
