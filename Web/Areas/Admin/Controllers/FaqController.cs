using Application.Services.Interfaces;
using Application.ViewModels.Faq.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageFaqs")]
    public class FaqController : BaseAdminController
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

        [Permission("FaqsList")]
        [HttpGet("/admin/faqs-list")]
        public async Task<IActionResult> FaqsList(AdminSideFilterFaqsViewModel filter)
        {
            var result = await _faqService.FilterFaqs(filter);
            if (result.IsFailure) return View(new AdminSideFilterFaqsViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewFaq")]
        [HttpGet("/admin/add-new-faq")]
        public IActionResult CreateNewFaq()
        {
            return View();
        }

        [Permission("CreateNewFaq")]
        [HttpPost("/admin/add-new-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewFaq(AdminSideCreateNewFaqViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _faqService.CreateNewFaq(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(FaqsList));
        }

        [Permission("UpdateFaq")]
        [HttpGet("/admin/edit-faq")]
        public async Task<IActionResult> UpdateFaq(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(FaqsList));
            }
            var result = await _faqService.GetFaqForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(FaqsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateFaq")]
        [HttpPost("/admin/edit-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFaq(AdminSideUpdateFaqViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _faqService.UpdateFaq(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(FaqsList));
        }

        [Permission("DeleteFaq")]
        [HttpPost("/admin/remove-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(FaqsList));
            }
            var result = await _faqService.SoftDeleteFaq(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(FaqsList));
        }
    }
}
