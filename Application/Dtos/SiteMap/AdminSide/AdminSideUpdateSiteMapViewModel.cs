using Application.Dtos;
using Domain.Enums.SiteInfo;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SiteMap.AdminSide
{
    public class AdminSideUpdateSiteMapViewModel : CommonDto
    {
        [Display(Name = "آدرس URL")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Url { get; set; }

        [Display(Name = "تغییر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public ChangeFreqEnum ChangeFreq { get; set; }

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
