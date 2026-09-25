using Application.Dtos.ContactUsForm.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ContactUsController : BaseController
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

        [HttpGet("/contact-us")]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost("/contact-us"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ClientSideContactUsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _formService.UserContactUs(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ContactUs));
        }
    }
}
