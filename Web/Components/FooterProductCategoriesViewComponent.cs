using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public FooterProductCategoriesViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetParentCategories();
            if (result.IsFailure) return View("FooterProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("FooterProductCategories", result.Value);
        }
    }
}
