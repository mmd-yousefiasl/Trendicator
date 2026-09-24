using Application.Dtos.ContactUsForm.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageContactUs")]
    public class ContactUsController : BaseAdminController
    {
        #region Fields
        private readonly IContactUsFormService _formService;
        #endregion

        #region Ctor
        public ContactUsController(IContactUsFormService formService)
        {
            _formService = formService;
        }
        #endregion

        [Permission("ContactUsList")]
        [HttpGet("/admin/contact-us-list")]
        public async Task<IActionResult> ContactUsList(AdminSideFilterContactUsFormsViewModel filter)
        {
            var result = await _formService.FilterContactUsForms(filter);
            if (result.IsFailure) return View(new AdminSideFilterContactUsFormsViewModel());

            return View(result.Value);
        }

        [Permission("DeleteContactUsForm")]
        [HttpPost("/admin/remove-contact-us-form"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContactUsForm(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ContactUsList));
            }
            var result = await _formService.SoftDeleteContactUsForm(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ContactUsList));
        }
    }
}
