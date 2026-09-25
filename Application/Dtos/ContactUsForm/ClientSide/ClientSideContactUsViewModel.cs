using Domain.Enums.ContactUs;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ContactUsForm.ClientSide
{
    public class ClientSideContactUsViewModel
    {
        [Display(Name = "نام و نام خانوادگی", Prompt = "نام و نام خانوادگی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string FullName { get; set; }

        [Display(Name = "شماره موبایل", Prompt = "09.........")]
        [MaxLength(15, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        [RegularExpression("^09\\d{9}$", ErrorMessage = Messages.MobileFormatError)]
        public string Mobile { get; set; }

        [Display(Name = "موضوع", Prompt = "موضوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public ContactUsFormTypeEnum Type { get; set; }

        [Display(Name = "پیام", Prompt = "پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        public string Message { get; set; }
    }
}
