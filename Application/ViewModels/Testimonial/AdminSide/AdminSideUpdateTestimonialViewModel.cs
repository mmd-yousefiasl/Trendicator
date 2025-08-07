using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Testimonial.AdminSide
{
    public class AdminSideUpdateTestimonialViewModel : CommonViewModel
    {
        [Display(Name = "گواهی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Content { get; set; }

        [Display(Name = "نویسنده")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string Writer { get; set; }

        [Display(Name = "موقعیت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string Position { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }
    }
}
