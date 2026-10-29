using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductFeature.AdminSide
{
    public class AdminSideCreateNewProductFeatureDto
    {
        [Display(Name = "متن")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Text { get; set; }

        [Display(Name = "محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RangeError)]
        public int ProductId { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }
    }
}
