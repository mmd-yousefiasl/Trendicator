using Application.Services.Interfaces;
using Application.ViewModels.ProductReview.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class ProductReviewsRateViewComponent : ViewComponent
    {
        private IProductReviewService _reviewService;
        public ProductReviewsRateViewComponent(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var result = await _reviewService.GetProductReviewRate(productId);
            if (result.IsFailure) return View("ProductReviewsRate", new ClientSideProductReviewRateDataViewModel() { });

            return View("ProductReviewsRate", result.Value);
        }
    }
}
