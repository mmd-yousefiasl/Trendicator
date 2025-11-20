using Application.Services.Interfaces;
using Application.ViewModels.ProductCategory.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class HeaderProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _categoryService;
        public HeaderProductCategoriesViewComponent(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _categoryService.ClientSideGetProductCategories();
            if (result.IsFailure) return View("HeaderProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("HeaderProductCategories", result.Value);
        }
    }
}
