using Application.Dtos.ProductFaq.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductFaqs")]
    public class ProductFaqController : BaseAdminController
    {
        #region Fields
        private readonly IProductFaqService _faqService;
        #endregion

        #region Ctor
        public ProductFaqController(IProductFaqService faqService)
        {
            _faqService = faqService;
        }
        #endregion

        [Permission("CreateNewProductFaq")]
        [HttpGet("/admin/add-new-product-faq")]
        public IActionResult CreateNewProductFaq(int id)
        {
            if (id < 1) return NotFound();

            return PartialView("_CreateNewProductFaq", new AdminSideCreateNewProductFaqViewModel() { ProductId = id });
        }

        [Permission("CreateNewProductFaq")]
        [HttpPost("/admin/add-new-product-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductFaq(AdminSideCreateNewProductFaqViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = Messages.OperationFailedError });
            }

            var result = await _faqService.AdminSideCreateNewProductFaq(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("UpdateProductFaq")]
        [HttpGet("/admin/edit-product-faq")]
        public async Task<IActionResult> UpdateProductFaq(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });
            var result = await _faqService.AdminSideGetProductFaqForUpdate(id);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return PartialView("_UpdateProductFaq", result.Value);
        }

        [Permission("UpdateProductFaq")]
        [HttpPost("/admin/edit-product-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductFaq(AdminSideUpdateProductFaqViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _faqService.AdminSideUpdateProductFaq(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("DeleteProductFaq")]
        [HttpPost("/admin/remove-product-faq"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductFaq(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });
            var result = await _faqService.AdminSideDeleteProductFaq(id);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }
    }
}
