using Application.Dtos.ProductDetails.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductDetails")]
    public class ProductDetailsController : BaseAdminController
    {
        #region Fields
        private readonly IProductService _productService;
        #endregion

        #region Ctor
        public ProductDetailsController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        [Permission("CreateNewProductDetails")]
        [HttpGet("/admin/add-new-product-details")]
        public IActionResult CreateNewProductDetails(int id)
        {
            if (id < 1)
                return NotFound();

            return PartialView("_CreateNewProductDetails", new AdminSideCreateNewProductDetailsDto() { ProductId = id, Priority = 50, IsPublished = true });
        }

        [Permission("CreateNewProductDetails")]
        [HttpPost("/admin/add-new-product-details"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductDetails(AdminSideCreateNewProductDetailsDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideCreateNewProductDeatils(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("UpdateProductDetails")]
        [HttpGet("/admin/edit-product-details")]
        public async Task<IActionResult> UpdateProductDetails(int id)
        {
            if (id < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideGetProductDetailsForUpdate(id);

            if (result.IsFailure)
                return Json(new { success = false, message = result.Message });

            return PartialView("_UpdateProductDetails", result.Value);
        }

        [Permission("UpdateProductDetails")]
        [HttpPost("/admin/edit-product-details"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductDetails(AdminSideUpdateProductDetailsDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideUpdateProductDetails(model);
            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        [Permission("DeleteProductDetails")]
        [HttpPost("/admin/remove-product-details"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductDetails(int id)
        {
            if (id < 1)
                return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _productService.AdminSideDeleteProductDetails(id);

            if (result.IsFailure)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }
    }
}