using Application.ValidationAttributes;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.ClientSide
{
    public class ClientSideLoginCodeDto
    {
        public string? ReturnUrl { get; set; }

        [Display(Name = "شماره موبایل", Prompt = "شماره موبایل")]
        [MaxLength(15, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        [RegularExpression("^09\\d{9}$", ErrorMessage = Messages.MobileFormatError)]
        public string Mobile { get; set; }

        [Display(Name = "کد تایید")]
        [RequiredEach(ErrorMessage = Messages.RequiredError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public List<string> LoginCode { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
