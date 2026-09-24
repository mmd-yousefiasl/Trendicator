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
        public async Task<IActionResult> OrdersList(AdminSideFilterCartsViewModel filter)
        {
            var result = await _cartService.AdminSideFilterCarts(filter);
            if (result.IsFailure) return View(new AdminSideFilterCartsViewModel());

            return View(result.Value);
        }

        [Permission("UpdateOrder")]
        [HttpGet("/admin/edit-order")]
        public async Task<IActionResult> UpdateOrder(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(OrdersList));
            }
            var result = await _cartService.AdminSideGetStatusForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(OrdersList));
            }

            return View(result.Value);
        }

        [Permission("UpdateOrder")]
        [HttpPost("/admin/edit-order"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(AdminSideUpdateCartStatusViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _cartService.AdminSideUpdateCartStatus(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(OrdersList));
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
    }
}
