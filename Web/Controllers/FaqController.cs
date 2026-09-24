using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class FaqController : BaseController
    {
        #region Fields
        private readonly IFaqService _faqService;
        #endregion

        #region Ctor
        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }
        #endregion

        [HttpGet("/faq")]
        public async Task<IActionResult> Faqs()
        {
            var result = await _faqService.GetFaqs();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(result.Value);
        }
    }
}
