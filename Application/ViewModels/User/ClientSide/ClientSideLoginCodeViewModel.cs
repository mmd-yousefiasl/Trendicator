using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User.ClientSide
{
    public class ClientSideLoginCodeViewModel
    {
        public string? ReturnUrl { get; set; }

        [Display(Name = "شماره موبایل", Prompt = "شماره موبایل")]
        [MaxLength(15, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        [RegularExpression("^09\\d{9}$", ErrorMessage = Messages.MobileFormatError)]
        public string PhoneNumber { get; set; }

        [Display(Name = "کد تایید", Prompt = "_ _ _ _")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(4, ErrorMessage = Messages.InvalidLoginCodeError)]
        [MinLength(4, ErrorMessage = Messages.InvalidLoginCodeError)]
        public string LoginCode { get; set; }

        [Display(Name = "من ربات نیستم")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Captcha { get; set; }
    }
}
