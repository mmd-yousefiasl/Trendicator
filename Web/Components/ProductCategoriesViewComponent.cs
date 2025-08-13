using Application.Services.Interfaces;
using Application.ViewModels.ProductCategory.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class ProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _categoryService;
        public ProductCategoriesViewComponent(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _categoryService.ClientSideGetProductCategories();
            if (result.IsFailure) return View("ProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("ProductCategories", result.Value);
        }
    }
}
