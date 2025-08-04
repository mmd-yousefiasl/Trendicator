using Application.Services.Interfaces;
using Application.ViewModels.User.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageUsers")]
    public class UserController : BaseAdminController
    {
        #region Fields
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        [Permission("UsersList")]
        [HttpGet("/admin/users-list")]
        public async Task<IActionResult> UsersList(AdminSideFilterUsersViewModel filter)
        {
            var result = await _userService.AdminSideFilterUsers(filter);
            if (result.IsFailure) return View(new AdminSideFilterUsersViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewUser")]
        [HttpGet("/admin/add-user")]
        public IActionResult CreateNewUser()
        {
            return View();
        }

        [Permission("CreateNewUser")]
        [HttpPost("/admin/add-user"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewUser(AdminSideCreateNewUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.CreateNewUser(model);

            if (result.IsFailure)
            {
                TempData[ErrorMessage] = result.Message;
                return View(model);
            }

            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UsersList));
        }

        [Permission("DeleteUser")]
        [HttpPost("/admin/remove-user"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(UsersList));
            }

            var result = await _userService.SoftDeleteUser(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(UsersList));
        }

        [Permission("UpdateUser")]
        [HttpGet("/admin/edit-user")]
        public async Task<IActionResult> UpdateUser(int id)
        {
            if (id < 1)
            {
                TempData[ErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(UsersList));
            }

            var result = await _userService.AdminSideGetUserForUpdate(id);
            if (result.IsFailure)
            {
                TempData[ErrorMessage] = result.Message;
                return RedirectToAction(nameof(UsersList));
            }

            return View(result.Value);
        }

        [Permission("UpdateUser")]
        [HttpPost("/admin/edit-user"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(AdminSideUpdateUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.UpdateUser(model);
            if (result.IsFailure)
            {
                TempData[ErrorMessage] = result.Message;
                return View(model);
            }

            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UsersList));
        }
    }
}
