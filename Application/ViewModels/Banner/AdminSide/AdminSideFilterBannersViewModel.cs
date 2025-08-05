using Domain.Enums.Banner;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Banner.AdminSide
{
    public class AdminSideFilterBannersViewModel : BasePaging<AdminSideBannerViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }

        [Display(Name = "نوع")]
        public BannerTypeEnum? Type { get; set; }
    }
}
