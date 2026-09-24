using Application.Dtos;
using Domain.Enums.SiteInfo;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SmsData.AdminSide
{
    public class AdminSideUpdateSmsPanelDto : CommonDto
    {
        [Display(Name = "کلید پنل پیامکی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string ApiKey { get; set; }

        [Display(Name = "الگوی قالب پیامک ورود")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int PatternId { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public SmsPanelTypeEnum Type { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsActive { get; set; }
    }
}
