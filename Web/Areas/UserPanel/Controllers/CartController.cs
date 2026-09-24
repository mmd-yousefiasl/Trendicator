using Application.Dtos.Cart.ClientSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class CartController : BaseUserPanelController
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

        [HttpGet("/user/my-orders")]
        public async Task<IActionResult> MyOrders(ClientSideFilterCartsViewModel filter)
        {
            var result = await _cartService.ClientSideFilterCarts(filter);
            if (result.IsFailure) return View(new ClientSideFilterCartsViewModel());

            return View(result.Value);
        }

        [HttpGet("/user/my-order-details/{id}")]
        public async Task<IActionResult> MyOrderDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(MyOrders));
            }
            var result = await _cartService.ClientSideGetCartDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(MyOrders));
            }

            return View(result.Value);
        }
    }
}
