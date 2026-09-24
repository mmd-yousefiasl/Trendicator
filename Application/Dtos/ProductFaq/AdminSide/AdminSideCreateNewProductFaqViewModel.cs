using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductFaq.AdminSide
{
    public class AdminSideCreateNewProductFaqViewModel
    {
        [Display(Name = "سوال")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Question { get; set; }

        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Answer { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RangeError)]
        public int ProductId { get; set; }
    }
}
