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
            ViewData["NoIndex"] = true;
            filter.TakeEntity = 9;
            var result = await _blogService.ClientSideFilterBlogs(filter);
            if (result.IsFailure) return View(new ClientSideFilterBlogsViewModel());

            return View(result.Value);
        }

        [HttpGet("/b/{id}/{slug}")]
        public async Task<IActionResult> BlogDetails(int id, string slug)
        {
            if (string.IsNullOrWhiteSpace(slug) || id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogsList));
            }
            var result = await _blogService.ClientSideGetBlogDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BlogsList));
            }

            return View(result.Value);
        }

        [HttpGet("/b/{id}")]
        public async Task<IActionResult> BlogDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogsList));
            }
            var result = await _blogService.ClientSideGetBlogDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BlogsList));
            }

            return View(result.Value);
        }
    }
}
