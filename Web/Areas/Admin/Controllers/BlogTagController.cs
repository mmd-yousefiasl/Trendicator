using Application.Dtos.BlogTag.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageBlogTags")]
    public class BlogTagController : BaseAdminController
    {
        #region Fields
        private readonly IBlogTagService _tagService;
        #endregion

        #region Ctor
        public BlogTagController(IBlogTagService tagService)
        {
            _tagService = tagService;
        }
        #endregion

        [Permission("BlogTagsList")]
        [HttpGet("/admin/blog-tags-list")]
        public async Task<IActionResult> BlogTagsList(AdminSideFilterBlogTagsViewModel filter)
        {
            var result = await _tagService.FilterBlogTags(filter);
            if (result.IsFailure) return View(new AdminSideFilterBlogTagsViewModel());

            return View(result.Value);
        }

        [Permission("CraeteNewBlogTag")]
        [HttpGet("/admin/add-new-blog-tag")]
        public IActionResult CraeteNewBlogTag()
        {
            return View();
        }

        [Permission("CraeteNewBlogTag")]
        [HttpPost("/admin/add-new-blog-tag"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CraeteNewBlogTag(AdminSideCreateNewBlogTagViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _tagService.CreateNewBlogTag(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BlogTagsList));
        }

        [Permission("UpdateBlogTag")]
        [HttpGet("/admin/edit-blog-tag")]
        public async Task<IActionResult> UpdateBlogTag(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogTagsList));
            }
            var result = await _tagService.GetBlogTagForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BlogTagsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateBlogTag")]
        [HttpPost("/admin/edit-blog-tag"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBlogTag(AdminSideUpdateBlogTagViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _tagService.UpdateBlogTag(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BlogTagsList));

        }

        [Permission("DeleteBlogTag")]
        [HttpPost("/admin/remove-blog-tag"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogTag(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BlogTagsList));
            }
            var result = await _tagService.SoftDeleteBlogTag(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(BlogTagsList));
        }
    }
}
