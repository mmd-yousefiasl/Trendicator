using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.EmailData.AdminSide
{
    public class AdminSideUpdateEmailDataViewModel : CommonDto
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string From { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Password { get; set; }


        [Display(Name = "نام نمایشی ایمیل")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string EmailDisplayName { get; set; }

        [Display(Name = "SMTP")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Smtp { get; set; }

        [Display(Name = "پورت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 10000, ErrorMessage = Messages.RangeError)]
        public int Port { get; set; }
    }
}
