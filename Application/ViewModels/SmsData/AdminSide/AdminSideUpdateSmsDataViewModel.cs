using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SmsData.AdminSide
{
    public class AdminSideUpdateSmsDataViewModel : CommonViewModel
    {
        [Display(Name = "کلید پنل پیامکی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string SmsPanelAPIKey { get; set; }

        [Display(Name = "الگوی قالب پیامک ورود")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeError)]
        public int SmsPatternIdForLoginCode { get; set; }
    }
}
