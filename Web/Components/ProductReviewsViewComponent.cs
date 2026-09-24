using Application.Dtos.ProductReview.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class ProductReviewsViewComponent : ViewComponent
    {
        private readonly IProductReviewService _reviewService;
        public ProductReviewsViewComponent(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var result = await _reviewService.ClientSideProductReviews(productId);
            if (result.IsFailure) return View("ProductReviews", new List<ClientSideProductReviewViewModel>());

            return View("ProductReviews", result.Value);
        }
    }
}
