using Application.Services.Interfaces;
using Application.ViewModels.ProductReview.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ProductReviewController : BaseController
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

        [HttpPost("/write-review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteReview(ClientSideWriteProductReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction("ProductDetails", "Product", new { slug = model.Slug });
            }
            var result = await _reviewService.ClientSideWriteProductReview(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = Messages.ReviewAddedSuccessfully;
            }
            return RedirectToAction("ProductDetails", "Product", new { slug = model.Slug });
        }
    }
}
