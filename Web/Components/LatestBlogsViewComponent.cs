using Application.Services.Interfaces;
using Application.ViewModels.Blog.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class LatestBlogsViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;
        public LatestBlogsViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _blogService.ClientSideGetLatestBlogs();
            if (result.IsFailure) return View("LatestBlogs", new ClientSideFilterBlogsViewModel());

            return View("LatestBlogs", result.Value);
        }
    }
}
