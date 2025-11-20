using Domain.Enums.SiteInfo;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteMap.AdminSide
{
    public class AdminSideUpdateSiteMapViewModel : CommonViewModel
    {
        [Display(Name = "آدرس URL")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Url { get; set; }

        [Display(Name = "تغییر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public ChangeFreqEnum ChagneFreq { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(0, 1, ErrorMessage = Messages.RangeError)]
        public decimal Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public SiteMapTypeEnum Type { get; set; }
    }
}
