using Application.Services.Interfaces;
using Application.ViewModels.Blog.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageBlogs")]
    public class BlogController : BaseAdminController
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

        [Permission("BlogsList")]
        [HttpGet("/admin/blogs-list")]
        public async Task<IActionResult> BlogsList(AdminSideFilterBlogsViewModel filter)
        {
            var result = await _blogService.AdminSideFilterBlogs(filter);
            if (result.IsFailure) return View(new AdminSideFilterBlogsViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewBlog")]
        [HttpGet("/admin/add-new-blog")]
        public IActionResult CreateNewBlog()
        {
            return View();
        }

        [Permission("CreateNewBlog")]
        [HttpPost("/admin/add-new-blog"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewBlog(AdminSideCreateNewBlogViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _blogService.CreateNewBlog(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BlogsList));
        }

        [Permission("UpdateBlog")]
        [HttpGet("/admin/edit-blog")]
        public async Task<IActionResult> UpdateBlog(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogsList));
            }
            var result = await _blogService.GetBlogForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BlogsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateBlog")]
        [HttpPost("/admin/edit-blog"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBlog(AdminSideUpdateBlogViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _blogService.UpdateBlog(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BlogsList));
        }

        [Permission("DeleteBlog")]
        [HttpPost("/admin/remove-blog"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogsList));
            }
            var result = await _blogService.SoftDeleteBlog(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }
            return RedirectToAction(nameof(BlogsList));
        }
    }
}
