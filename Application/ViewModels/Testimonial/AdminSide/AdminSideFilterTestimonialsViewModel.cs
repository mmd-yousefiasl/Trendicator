using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Testimonial.AdminSide
{
    public class AdminSideFilterTestimonialsViewModel : BasePaging<AdminSideTestimonialViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
