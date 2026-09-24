using Application.Dtos.NewsLetter.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageNewsLetters")]
    public class NewsLettersController : BaseAdminController
    {
        #region Fields
        private readonly INewsLetterService _newsLettersService;
        #endregion

        #region Ctor
        public NewsLettersController(INewsLetterService newsLettersService)
        {
            _newsLettersService = newsLettersService;
        }
        #endregion

        [Permission("NewsLettersList")]
        [HttpGet("/admin/news-letters-list")]
        public async Task<IActionResult> NewsLettersList(AdminSideFilterNewsLettersViewModel filter)
        {
            var result = await _newsLettersService.FilterNewsLetters(filter);
            if (result.IsFailure) return View(new AdminSideFilterNewsLettersViewModel());
            return View(result.Value);
        }

        [Permission("DeleteNewsLetters")]
        [HttpPost("/admin/remove-news-letters"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsLetters(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(NewsLettersList));
            }
            var result = await _newsLettersService.SoftDeleteNewsLetter(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(NewsLettersList));
        }
    }
}
