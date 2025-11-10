using Application.ValidationAttributes;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User.ClientSide
{
    public class ClientSideLoginViewModel
    {
        public string? ReturnUrl { get; set; }

        [Display(Name = "شماره موبایل")]
        [MaxLength(15, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        [RegularExpression("^09\\d{9}$", ErrorMessage = Messages.MobileFormatError)]
        public string PhoneNumber { get; set; }

        [Display(Name = "سیاست حفظ حریم خصوصی و شرایط و ظوابط")]
        [MustBeTrue(ErrorMessage = Messages.RequiredError)]
        public bool PrivacyPolicy { get; set; }
    }
}
