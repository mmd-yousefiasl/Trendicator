using Application.Dtos.Media;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageMedia")]
    public class MediaController : BaseAdminController
    {
        #region Fields
        private readonly IMediaService _mediaService;
        #endregion

        #region Ctor
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }
        #endregion

        [Permission("MediaList")]
        [HttpGet("/admin/media-list")]
        public async Task<IActionResult> MediaList(AdminSideFilterMediaViewModel filter)
        {
            var result = await _mediaService.AdminSideFilterMedia(filter);
            if (result.IsFailure) return PartialView("_MediaList", new AdminSideFilterMediaViewModel());

            return PartialView("_MediaList", result.Value);
        }

        [Permission("CreateNewMedia")]
        [HttpPost("/admin/add-new-media"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewMedia(AdminSideCreateNewMediaViewModel model)
        {
            var result = await _mediaService.AdminSideCreateNewMedia(model);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }

        [Permission("DeleteMedia")]
        [HttpPost("/admin/remove-media"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var result = await _mediaService.AdminSideDeleteMedia(id);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }
    }
}
