using Application.Services.Interfaces;
using Application.ViewModels.PaymentData;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManagePaymentData")]
    public class PaymentDataController : BaseAdminController
    {
        #region Fields
        private readonly IPaymentDataService _paymentDataService;
        #endregion

        #region Ctor
        public PaymentDataController(IPaymentDataService paymentDataService)
        {
            _paymentDataService = paymentDataService;
        }
        #endregion

        [Permission("UpdatePaymentData")]
        [HttpGet("/admin/edit-payment-data")]
        public async Task<IActionResult> UpdatePaymentData()
        {
            var result = await _paymentDataService.AdminSideGetPaymentDataForUpdate();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View(result.Value);
        }

        [Permission("UpdatePaymentData")]
        [HttpPost("/admin/edit-payment-data"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePaymentData(AdminSideUpdatePaymentDataViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _paymentDataService.AdminSideUpdatePaymentData(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UpdatePaymentData));
        }
    }
}
