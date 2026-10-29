using Application.Dtos.ProductFeature.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductFeatures")]
    public class ProductFeatureController : BaseAdminController
    {
        #region Fields
        private readonly IProductService _productService;
        #endregion

        #region Ctor
        public ProductFeatureController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        [Permission("CreateNewProductFeature")]
        [HttpGet("/admin/add-new-product-feature")]
        public IActionResult CreateNewProductFeature(int id)
        {
            if (id < 1)
                return NotFound();

            return PartialView("_CreateNewProductFeature", new AdminSideCreateNewProductFeatureDto() { ProductId = id, Priority = 50, IsPublished = true });
        }

        [Permission("CreateNewProductFeature")]
        [HttpPost("/admin/add-new-product-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductFeature(AdminSideCreateNewProductFeatureDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideCreateNewProductFeature(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("UpdateProductFeature")]
        [HttpGet("/admin/edit-product-feature")]
        public async Task<IActionResult> UpdateProductFeature(int id)
        {
            if (id < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideGetProductFeatureForUpdate(id);

            if (result.IsFailure)
                return Json(new { success = false, message = result.Message });

            return PartialView("_UpdateProductFeature", result.Value);
        }

        [Permission("UpdateProductFeature")]
        [HttpPost("/admin/edit-product-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductFeature(AdminSideUpdateProductFeatureDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideUpdateProductFeature(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("DeleteProductFeature")]
        [HttpPost("/admin/remove-product-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductFeature(int id)
        {
            if (id < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideDeleteProductFeature(id);

            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }
    }
}
