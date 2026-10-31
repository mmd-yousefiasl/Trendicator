using Application.Dtos.ProductReview.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductReviews")]
    public class ProductReviewController : BaseAdminController
    {
        #region Fields
        private readonly IProductService _productService;
        #endregion

        #region Ctor
        public ProductReviewController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        [Permission("ProductReviewsList")]
        [HttpGet("/admin/product-reviews-list")]
        public async Task<IActionResult> ProductReviewsList(AdminSideFilterProductReviewsDto filter)
        {
            var result = await _productService.AdminSideFilterProductReviews(filter);

            if (result.IsFailure)
                return View(new AdminSideFilterProductReviewsDto());

            return View(result.Value);
        }

        [Permission("ConfirmProductReview")]
        [HttpPost("/admin/confirm-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmProductReview(int id, [Bind(Prefix = "")] AdminSideFilterProductReviewsDto filter)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList), filter);
            }

            var result = await _productService.AdminSideConfirmReview(id);
            if (result.IsFailure)
                TempData[TemporaryErrorMessage] = result.Message;
            else
                TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ProductReviewsList), filter);
        }

        [Permission("RejectProductReview")]
        [HttpPost("/admin/reject-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectProductReview(int id, [Bind(Prefix = "")] AdminSideFilterProductReviewsDto filter)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList));
            }
            var result = await _productService.AdminSideRejectReview(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductReviewsList), filter);
        }

        [Permission("DeleteProductReview")]
        [HttpPost("/admin/remove-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductReview(int id, [Bind(Prefix = "")] AdminSideFilterProductReviewsDto filter)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList));
            }
            var result = await _productService.AdminSideSoftDeleteProductReview(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductReviewsList), filter);
        }
    }
}