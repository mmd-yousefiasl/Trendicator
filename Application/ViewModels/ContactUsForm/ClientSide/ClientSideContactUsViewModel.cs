using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ContactUsForm.ClientSide
{
    public class ClientSideContactUsViewModel
    {
        [Display(Name = "نام کامل", Prompt = "نام کامل")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string FullName { get; set; }

        [Display(Name = "ایمیل", Prompt = "ایمیل")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        [EmailAddress(ErrorMessage = Messages.RequiredError)]
        public string Email { get; set; }

        [Display(Name = "تلفن", Prompt = "تلفن")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string PhoneNumber { get; set; }

        [Display(Name = "موضوع", Prompt = "موضوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Subject { get; set; }

        [Display(Name = "پیام", Prompt = "پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        public string Message { get; set; }
    }
}
