using Application.Dtos.ProductCategory.ClientSide;
using Application.Services.Interfaces;
using Application.Dtos.Product.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MobileHeaderProductCategoriesViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _categoryServce;
        public MobileHeaderProductCategoriesViewComponent(IProductCategoryService categoryService)
        {
            _categoryServce = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _categoryServce.ClientSideGetProductCategories();
            if (result.IsFailure) return View("MobileHeaderProductCategories", new List<ClientSideProductCategoryViewModel>());

            return View("MobileHeaderProductCategories", result.Value);
        }
    }
}
