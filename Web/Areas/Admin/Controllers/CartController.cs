using Application.Dtos.Cart.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageOrders")]
    public class CartController : BaseAdminController
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

        [Permission("OrdersList")]
        [HttpGet("/admin/orders-list")]
        public async Task<IActionResult> OrdersList(AdminSideFilterCartsDto filter)
        {
            var result = await _cartService.AdminSideFilterCarts(filter);
            if (result.IsFailure) return View(new AdminSideFilterCartsDto());

            return View(result.Value);
        }

        [Permission("OrderDetails")]
        [HttpGet("/admin/order-details/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(OrdersList));
            }
            var result = await _cartService.AdminSideGetCartDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(OrdersList));
            }

            return View(result.Value);
        }

        [Permission("OrderDetails")]
        [HttpPost("/admin/update-cart-item-status"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItemStatus(bool done, int cartId, int productId)
        {
            var result = await _cartService.AdminSideUpdateCartItemStatus(done, cartId, productId);

            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(OrderDetails), new { area = "Admin", id = cartId });
        }
    }
}
