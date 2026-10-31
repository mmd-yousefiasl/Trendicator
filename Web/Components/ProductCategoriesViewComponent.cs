using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class ProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public ProductCategoriesViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetProductCategories();
            if (result.IsFailure) return View("ProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("ProductCategories", result.Value);
        }
    }
}
