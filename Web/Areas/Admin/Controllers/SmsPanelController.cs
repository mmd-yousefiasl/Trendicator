using Application.Dtos.SmsData.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageSmsPanels")]
    public class SmsPanelController : BaseAdminController
    {
        #region Fields
        private readonly ISmsPanelService _smsPanelService;
        #endregion

        #region Ctor
        public SmsPanelController(ISmsPanelService smsPanelService)
        {
            _smsPanelService = smsPanelService;
        }
        #endregion

        [Permission("SmsPanelsList")]
        [HttpGet("/admin/sms-panels-list")]
        public async Task<IActionResult> SmsPanelsList(AdminSideFilterSmsPanelsDto filter)
        {
            var result = await _smsPanelService.AdminSideFilterSmsPanles(filter);
            if (result.IsFailure)
                return View(new AdminSideFilterSmsPanelsDto());

            return View(result.Value);
        }

        [Permission("CreateNewSmsPanel")]
        [HttpGet("/admin/add-sms-panel")]
        public IActionResult CreateNewSmsPanel()
        {
            return View();
        }

        [Permission("CreateNewSmsPanel")]
        [HttpPost("/admin/add-sms-panel"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewSmsPanel(AdminSideCreateNewSmsPanelDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _smsPanelService.AdminSideCreateNewSmsPanel(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(dto);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(SmsPanelsList));
        }

        [Permission("UpdateSmsPanel")]
        [HttpGet("/admin/edit-sms-panel")]
        public async Task<IActionResult> UpdateSmsPanel(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(SmsPanelsList));
            }

            var result = await _smsPanelService.AdminSideGetSmsPanelForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(SmsPanelsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateSmsPanel")]
        [HttpPost("/admin/edit-sms-panel"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSmsPanel(AdminSideUpdateSmsPanelDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _smsPanelService.AdminSideUpdateSmsPanel(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(dto);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(SmsPanelsList));
        }

        [Permission("DeleteSmsPanel")]
        [HttpPost("/admin/remove-sms-panel"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSmsPanel(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(SmsPanelsList));
            }

            var result = await _smsPanelService.AdminSideDeleteSmsPanel(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(SmsPanelsList));
        }
    }
}
