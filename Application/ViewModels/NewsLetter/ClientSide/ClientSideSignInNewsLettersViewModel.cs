using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.NewsLetter.ClientSide
{
    public class ClientSideSignInNewsLettersViewModel
    {
        [Display(Name = "ایمیل", Prompt = "ایمیل خود را وارد کنید")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        [EmailAddress(ErrorMessage = Messages.RequiredError)]
        public string Email { get; set; }
    }
}
