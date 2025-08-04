using Application.Services.Interfaces;
using Application.ViewModels.Role.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageRoles")]
    public class RoleController : BaseAdminController
    {
        #region Fields
        private readonly IRoleService _roleService;
        #endregion

        #region Ctor
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        #endregion

        [Permission("RolesList")]
        [HttpGet("/admin/roles-list")]
        public async Task<IActionResult> RolesList(AdminSideFilterRolesViewModel filter)
        {
            var result = await _roleService.AdminSideFilterRoles(filter);
            if (result.IsFailure) return View(new AdminSideFilterRolesViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewRole")]
        [HttpGet("/admin/add-role")]
        public IActionResult CreateNewRole()
        {
            return View();
        }

        [Permission("CreateNewRole")]
        [HttpPost("/admin/add-role"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewRole(AdminSideCreateNewRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _roleService.AdminSideCreateNewRole(model);
            if (result.IsFailure)
            {
                TempData[ErrorMessage] = result.Message;
                return View(model);
            }

            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(RolesList));
        }

        [Permission("DeleteRole")]
        [HttpPost("/admin/remove-role"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(RolesList));
            }

            var result = await _roleService.AdminSideSoftDeleteRole(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(RolesList));
        }

        [Permission("UpdateRole")]
        [HttpGet("/admin/edit-role")]
        public async Task<IActionResult> UpdateRole(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(RolesList));
            }
            var result = await _roleService.AdminSideGetRoleForUpdate(id);

            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(RolesList));
            }

            return View(result.Value);
        }

        [Permission("UpdateRole")]
        [HttpPost("/admin/edit-role"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(AdminSideUpdateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _roleService.AdminSideUpdateRole(model);

            if (result.IsFailure)
            {
                TempData[ErrorMessage] = result.Message;
                return View(model);
            }

            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(RolesList));
        }
    }
}
