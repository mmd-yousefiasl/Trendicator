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
        private readonly IProductReviewService _reviewService;
        #endregion

        #region Ctor
        public ProductReviewController(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        #endregion

        [Permission("ProductReviewsList")]
        [HttpGet("/admin/product-reviews-list")]
        public async Task<IActionResult> ProductReviewsList(AdminSideFilterProductReviewsViewModel filter)
        {
            var result = await _reviewService.AdminSideFilterProductReviews(filter);
            if (result.IsFailure) return View(new AdminSideFilterProductReviewsViewModel());

            return View(result.Value);
        }

        [Permission("ConfirmProductReview")]
        [HttpPost("/admin/confirm-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmProductReview(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList));
            }
            var result = await _reviewService.ConfirmReview(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductReviewsList));
        }

        [Permission("RejectProductReview")]
        [HttpPost("/admin/reject-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectProductReview(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList));
            }
            var result = await _reviewService.RejectReview(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductReviewsList));
        }

        [Permission("DeleteProductReview")]
        [HttpPost("/admin/remove-product-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductReview(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductReviewsList));
            }
            var result = await _reviewService.SoftDeleteProductReview(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductReviewsList));
        }
    }
}
