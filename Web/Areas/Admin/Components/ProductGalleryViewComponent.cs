using Application.Services.Interfaces;
using Application.ViewModels.ProductGallery.AdminSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductGalleryViewComponent : ViewComponent
    {
        private readonly IProductGalleryService _galleryService;
        public ProductGalleryViewComponent(IProductGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _galleryService.AdminSideGetProductGalleries(id);
            if (result.IsFailure) return View("ProductGallery", new List<AdminSideProductGalleryViewModel>());

            return View("ProductGallery", result.Value);
        }
    }
}
