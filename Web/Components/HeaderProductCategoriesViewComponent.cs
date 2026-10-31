using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class HeaderProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public HeaderProductCategoriesViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetProductCategories();
            if (result.IsFailure) return View("HeaderProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("HeaderProductCategories", result.Value);
        }
    }
}
