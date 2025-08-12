using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductGallery.AdminSide
{
    public class AdminSideCreateNewProductGalleryViewModel
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public IFormFile File { get; set; }

        [Display(Name = "محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int ProductId { get; set; }
    }
}
