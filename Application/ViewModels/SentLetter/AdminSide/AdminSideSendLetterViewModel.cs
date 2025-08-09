using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SentLetter.AdminSide
{
    public class AdminSideSendLetterViewModel
    {
        [Display(Name = "موضوع")]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Subject { get; set; }

        [Display(Name = "پیام")]
        [MaxLength(5000, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Message { get; set; }
    }
}
