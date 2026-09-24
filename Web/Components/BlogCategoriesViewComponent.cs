using Application.Dtos.BlogCategory.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace Web.Components
{
    public class BlogCategoriesViewComponent : ViewComponent
    {
        private readonly IBlogCategoryService _categoryService;
        public BlogCategoriesViewComponent(IBlogCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _categoryService.ClientSideGetBlogCategories();
            if (result.IsFailure) return View("BlogCategories", new List<ClientSideBlogCategoryViewModel>());

            return View("BlogCategories", result.Value);
        }
    }
}
