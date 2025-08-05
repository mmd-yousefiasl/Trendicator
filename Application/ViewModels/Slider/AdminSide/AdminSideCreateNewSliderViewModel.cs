using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Slider.AdminSide
{
    public class AdminSideCreateNewSliderViewModel
    {
        [Display(Name = "بنر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public IFormFile BannerFile { get; set; }

        [Display(Name = "عنوان کوچک")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string SubTitle { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "عنوان دکمه")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string ButtonTitle { get; set; }

        [Display(Name = "لینک")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Url { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }
    }
}
