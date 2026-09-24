using Application.Dtos.EmailData.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageEmailData")]
    public class EmailDataController : BaseAdminController
    {
        #region Fields
        private readonly IEmailDataService _emailDataService;
        #endregion

        #region Ctor
        public EmailDataController(IEmailDataService emailDataService)
        {
            _emailDataService = emailDataService;
        }
        #endregion

        [Permission("UpdateEmailData")]
        [HttpGet("/admin/edit-email-data")]
        public async Task<IActionResult> UpdateEmailData()
        {
            var result = await _emailDataService.GetEmailDataForUpdate();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View(result.Value);
        }

        [Permission("UpdateEmailData")]
        [HttpPost("/admin/edit-email-data"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmailData(AdminSideUpdateEmailDataViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _emailDataService.UpdateEmailData(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UpdateEmailData));
        }
    }
}
