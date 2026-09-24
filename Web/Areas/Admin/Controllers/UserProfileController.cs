using Application.Dtos.User.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    public class UserProfileController : BaseAdminController
    {
        #region Fields
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public UserProfileController(IUserService userService)
        {
            _userService = userService;
            _userService = userService;
        }
        #endregion

        [HttpGet("/admin/edit-user-profile")]
        public async Task<IActionResult> UpdateUserProfile()
        {
            var result = await _userService.AdminSideGetUserProfileForUpdate();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View(result.Value);
        }

        [HttpPost("/admin/edit-user-profile"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserProfile(AdminSideUpdateUserProfileDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.AdminSideUpdateUserProfile(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
