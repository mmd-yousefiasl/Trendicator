using Domain.Enums.SiteInfo;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteMap.AdminSide
{
    public class AdminSideFilterSiteMapViewModel : BasePaging<AdminSideSiteMapViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "تغییر")]
        public ChangeFreqEnum? ChangeFreq { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }

        [Display(Name = "نوع")]
        public SiteMapTypeEnum? Type { get; set; }
    }
}
