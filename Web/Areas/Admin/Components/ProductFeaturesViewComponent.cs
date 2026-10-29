using Application.Dtos.ProductFeature.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductFeaturesViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public ProductFeaturesViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _productService.AdminSideFilterProductFeatures(new AdminSideFilterProductFeaturesDto() { ProductId = id, TakeEntity = 50 });
            if (result.IsFailure)
                return View("ProductFeatures", new AdminSideFilterProductFeaturesDto() { ProductId = id });

            return View("ProductFeatures", result.Value);
        }
    }
}
