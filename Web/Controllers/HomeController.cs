using Application.Services.Interfaces;
using Application.ViewModels.NewsLetter.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Fields
        private readonly INewsLetterService _newsLettersService;
        #endregion

        #region Ctor
        public HomeController(INewsLetterService newsLettersService)
        {
            _newsLettersService = newsLettersService;
        }
        #endregion

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/sign-in-news-letters"), ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInNewsLetters(ClientSideSignInNewsLettersViewModel model)
        {
            if (!ModelState.IsValid) return View(nameof(Index), model);

            var result = await _newsLettersService.SignInNewsLetters(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(nameof(Index), model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [HttpGet("/403")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("/500")]
        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}
