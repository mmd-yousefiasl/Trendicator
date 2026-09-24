using Application.Dtos;
using Domain.Enums.SiteInfo;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SocialMedia.AdminSide
{
    public class AdminSideUpdateSocialMediaViewModel : CommonDto
    {
        [Display(Name = "لینک")]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Url { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public SocialMediaTypeEnum Type { get; set; }
    }
}
