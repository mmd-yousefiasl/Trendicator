using Application.Services.Interfaces;
using Application.ViewModels.ProductGallery.AdminSide;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductGallery")]
    public class ProductGalleryController : BaseAdminController
    {
        #region Fields
        private readonly IProductGalleryService _galleryService;
        #endregion

        #region Ctor
        public ProductGalleryController(IProductGalleryService galleryService)
        {
            _galleryService = galleryService;
        }
        #endregion

        [Permission("ProductGalleryList")]
        [HttpGet("/admin/product-gallery-list")]
        public IActionResult ProductGalleryList(int id)
        {
            return PartialView("_ProductGalleryList", new AdminSideCreateNewProductGalleryViewModel() { ProductId = id });
        }

        [Permission("CreateNewProductGallery")]
        [HttpPost("/admin/add-new-product-gallery"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductGallery(AdminSideCreateNewProductGalleryViewModel model)
        {
            var result = await _galleryService.CreateNewProductGallery(model);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }

        [Permission("DeleteProductGallery")]
        [HttpPost("/admin/remove-product-gallery"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductGallery(int id)
        {
            var result = await _galleryService.DeleteProductGallery(id);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }
    }
}
