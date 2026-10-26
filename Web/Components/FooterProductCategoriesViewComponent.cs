using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _categoryService;
        public FooterProductCategoriesViewComponent(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _categoryService.ClientSideGetParentCategories();
            if (result.IsFailure) return View("FooterProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("FooterProductCategories", result.Value);
        }
    }
}
