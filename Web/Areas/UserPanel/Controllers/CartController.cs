using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
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
        public async Task<IActionResult> MyOrders(ClientSideFilterCartsDto filter)
        {
            var result = await _cartService.ClientSideFilterCarts(filter);

            if (result.IsFailure)
                return View(new ClientSideFilterCartsDto());

            return View(result.Value);
        }

        [HttpGet("/user/my-order-details/{id}")]
        public async Task<IActionResult> MyOrderDetails(int id)
        {
            var result = await _cartService.ClientSideGetCartDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(MyOrders));
            }

            return View(result.Value);
        }

        [HttpGet("/user/my-products")]
        public async Task<IActionResult> MyProducts(ClientSideFilterMyProductsDto filter)
        {
            var result = await _cartService.ClientSideFilterPurchasedProducts(filter);

            if (result.IsFailure)
                return View(new ClientSideFilterMyProductsDto());

            return View(result.Value);
        }

        [HttpPost("/user/download-my-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadMyFile(int cartId, int productId)
        {
            var result = await _cartService.ClientSideGetFileInfo(cartId, productId);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(MyProducts));
            }

            if (!System.IO.File.Exists(result.Value.FileAddress))
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(MyProducts));
            }
            var stream = new FileStream(result.Value.FileAddress, FileMode.Open, FileAccess.Read);
            return File(stream, "application/zip", result.Value.FileName + ".zip");
        }

        [HttpPost("/user/activate-my-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateMyProduct(int cartId, int productId)
        {
            var result = await _cartService.ClientSideGetMetaTraderLink(cartId, productId);

            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(MyProducts));
            }

            return Redirect(result.Value.ToString()!);
        }
    }
}
