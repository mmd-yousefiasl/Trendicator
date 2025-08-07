using Application.Services.Interfaces;
using Application.ViewModels.Blog.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class RelatedBlogsViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;
        public RelatedBlogsViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id, int categoryId)
        {
            var result = await _blogService.ClientSideGetRelatedBlogs(id, categoryId);
            if (result.IsFailure) return View("RelatedBlogs", new ClientSideFilterBlogsViewModel());

            return View("RelatedBlogs", result.Value);
        }
    }
}
