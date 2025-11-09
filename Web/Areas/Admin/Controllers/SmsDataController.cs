using Application.Services.Interfaces;
using Application.ViewModels.SmsData.AdminSide;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageSmsData")]
    public class SmsDataController : BaseAdminController
    {
        #region Fields
        private readonly ISmsDataService _smsService;
        #endregion

        #region Ctor
        public SmsDataController(ISmsDataService smsService)
        {
            _smsService = smsService;
        }
        #endregion

        [Permission("UpdateSmsData")]
        [HttpGet("/admin/edit-sms-data")]
        public async Task<IActionResult> UpdateSmsData()
        {
            var result = await _smsService.AdminSideGetSmsDataForUpdate();
            if (result.IsFailure) return View(new AdminSideUpdateSmsDataViewModel());

            return View(result.Value);
        }

        [Permission("UpdateSmsData")]
        [HttpPost("/admin/edit-sms-data"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSmsData(AdminSideUpdateSmsDataViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _smsService.AdminSideUpdateSmsData(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UpdateSmsData));
        }
    }
}
