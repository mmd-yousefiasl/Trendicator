using Application.Services.Interfaces;
using Application.ViewModels.SentLetter.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageNewsLetters")]
    public class SentLetterController : BaseAdminController
    {
        #region Fields
        private readonly ISentLetterService _sentLetterService;
        private readonly INewsLetterService _newsLettersService;
        #endregion

        #region Ctor
        public SentLetterController(ISentLetterService sentLetterService,
            INewsLetterService newsLettersService)
        {
            _sentLetterService = sentLetterService;
            _newsLettersService = newsLettersService;
        }
        #endregion

        [Permission("SentLettersList")]
        [HttpGet("/admin/sent-letters-list")]
        public async Task<IActionResult> SentLettersList(AdminSideFilterSentLettersViewModel filter)
        {
            var result = await _sentLetterService.AdminSideFilterSentLetters(filter);
            if (result.IsFailure) return View(new AdminSideFilterSentLettersViewModel());

            return View(result.Value);
        }

        [Permission("DeleteSentLetter")]
        [HttpPost("/admin/remove-sent-letter"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSentLetter(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(SentLettersList));
            }

            var result = await _sentLetterService.DeleteSentLetter(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(SentLettersList));
        }

        [Permission("SendLetter")]
        [HttpGet("/admin/send-letter")]
        public IActionResult SendLetter()
        {
            return View();
        }

        [Permission("SendLetter")]
        [HttpPost("/admin/send-letter"), ValidateAntiForgeryToken]
        public async Task<IActionResult> SendLetter(AdminSideSendLetterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _newsLettersService.SendNews(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(SentLettersList));
        }
    }
}
