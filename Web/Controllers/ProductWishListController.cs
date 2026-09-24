using Application.Dtos.ProductWishList.ClientSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ProductWishListController : BaseController
    {
        #region Fields
        private readonly IProductWishListService _wishListService;
        #endregion

        #region Ctor
        public ProductWishListController(IProductWishListService wishListService)
        {
            _wishListService = wishListService;
        }
        #endregion

        [HttpPost("/product/toggle-wishlist")]
        public async Task<IActionResult> UpdateWishList([FromBody] ClientSideUpdateProductWishListViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            switch (model.Action)
            {
                case "Add":
                    await _wishListService.AddProductToWishList(model.ProductId);
                    break;
                case "Remove":
                    await _wishListService.RemoveProductFromWishList(model.ProductId);
                    break;
                default:
                    return BadRequest();
            }

            return Ok();
        }

        [HttpGet("/product/wishlist")]
        public async Task<IActionResult> WishList(ClientSideFilterProductWishListViewModel filter)
        {
            ViewData["NoIndex"] = true;
            var result = await _wishListService.ClientSideFilterWishList(filter);
            if (result.IsFailure) return View(new ClientSideFilterProductWishListViewModel());

            return View(result.Value);
        }

        [HttpGet("/product/remove-wishlist")]
        public async Task<IActionResult> DeleteWishList(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(WishList));
            }
            var result = await _wishListService.DeleteWishList(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(WishList));
        }
    }
}
