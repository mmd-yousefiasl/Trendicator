using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MobileHeaderProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public MobileHeaderProductCategoriesViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetProductCategories();
            if (result.IsFailure) return View("MobileHeaderProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("MobileHeaderProductCategories", result.Value);
        }
    }
}
