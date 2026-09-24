using Application.Dtos.BlogTag.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class BlogTagsViewComponent : ViewComponent
    {
        private readonly IBlogTagService _tagService;
        public BlogTagsViewComponent(IBlogTagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _tagService.ClientSideGetBlogTags();
            if (result.IsFailure) return View("BlogTags", new List<ClientSideBlogTagViewModel>());

            return View("BlogTags", result.Value);
        }
    }
}
