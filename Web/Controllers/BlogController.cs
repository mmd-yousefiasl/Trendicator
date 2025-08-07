using Application.Services.Interfaces;
using Application.ViewModels.Blog.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BlogController : BaseController
    {
        #region Fields
        private readonly IBlogService _blogService;
        #endregion

        #region Ctor
        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        #endregion

        [HttpGet("/blogs")]
        public async Task<IActionResult> BlogsList(ClientSideFilterBlogsViewModel filter)
        {
            filter.TakeEntity = 9;
            var result = await _blogService.ClientSideFilterBlogs(filter);
            if (result.IsFailure) return View(new ClientSideFilterBlogsViewModel());

            return View(result.Value);
        }

        [HttpGet("/blog/{slug}")]
        public async Task<IActionResult> BlogDetails(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogsList));
            }
            var result = await _blogService.ClientSideGetBlogDetails(slug);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BlogsList));
            }

            return View(result.Value);
        }
    }
}
