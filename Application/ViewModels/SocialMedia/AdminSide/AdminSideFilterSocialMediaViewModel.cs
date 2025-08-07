using Domain.Enums.SiteInfo;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SocialMedia.AdminSide
{
    public class AdminSideFilterSocialMediaViewModel : BasePaging<AdminSideSocialMediaViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }

        [Display(Name = "نوع")]
        public SocialMediaTypeEnum? Type { get; set; }
    }
}
