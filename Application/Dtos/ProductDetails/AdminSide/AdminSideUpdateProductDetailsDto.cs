using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductDetails.AdminSide
{
    public class AdminSideUpdateProductDetailsDto : CommonDto
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Description { get; set; }

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
