using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class TermsAndConditionsController : BaseController
    {
        #region Fields
        private readonly ISiteDataService _dataService;
        #endregion

        #region Ctor
        public TermsAndConditionsController(ISiteDataService dataService)
        {
            _dataService = dataService;
        }
        #endregion

        [HttpGet("/terms-and-conditions")]
        public async Task<IActionResult> TermsAndConditions()
        {
            var result = await _dataService.ClientSideGetTermsAndConditions();
            if (result.IsFailure) return View("TermsAndConditions", "");

            return View("TermsAndConditions", result.Value);
        }

        [HttpGet("/privacy-policy")]
        public async Task<IActionResult> PrivacyPolicy()
        {
            var result = await _dataService.ClientSideGetPrivacyPolicy();
            if (result.IsFailure) return View("PrivacyPolicy", "");

            return View("PrivacyPolicy", result.Value);
        }
    }
}
