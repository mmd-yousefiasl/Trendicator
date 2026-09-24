using Application.Dtos.ProductOption.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductOptions")]
    public class ProductOptionController : BaseAdminController
    {
        #region Fields
        private readonly IProductOptionService _optionService;
        #endregion

        #region Ctor
        public ProductOptionController(IProductOptionService optionService)
        {
            _optionService = optionService;
        }
        #endregion

        [Permission("CraeteNewProductOption")]
        [HttpGet("/admin/add-product-option")]
        public IActionResult CraeteNewProductOption(int id)
        {
            if (id < 1) return NotFound();

            return PartialView("_CreateNewProductOption", new AdminSideCreateNewProductOptionViewModel() { ProductId = id });
        }

        [Permission("CraeteNewProductOption")]
        [HttpPost("/admin/add-product-option"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductStock(AdminSideCreateNewProductOptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = Messages.OperationFailedError });
            }

            var result = await _optionService.AdminSideCreateNewProductOption(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("UpdateProductOption")]
        [HttpGet("/admin/edit-product-option")]
        public async Task<IActionResult> UpdateProductOption(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });
            var result = await _optionService.AdminSideGetProductOptionForUpdate(id);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return PartialView("_UpdateProductOption", result.Value);
        }

        [Permission("UpdateProductOption")]
        [HttpPost("/admin/edit-product-option"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductOption(AdminSideUpdateProductOptionViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _optionService.AdminSideUpdateProductOption(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("DeleteProductOption")]
        [HttpPost("/admin/remove-product-option"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductOption(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });
            var result = await _optionService.AdminSideDeleteProductOption(id);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }
    }
}
