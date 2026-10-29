using Application.Dtos.ProductDetails.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductDetailsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public ProductDetailsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _productService.AdminSideFilterProductDetails(new AdminSideFilterProductDetailsDto() { ProductId = id, TakeEntity = 50 });
            if (result.IsFailure)
                return View("ProductDetails", new AdminSideFilterProductDetailsDto() { ProductId = id });

            return View("ProductDetails", result.Value);
        }
    }
}
