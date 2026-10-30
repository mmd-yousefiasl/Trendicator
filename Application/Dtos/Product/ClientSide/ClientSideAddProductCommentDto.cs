using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Product.ClientSide
{
    public class ClientSideAddProductCommentDto
    {
        [Display(Name = "شناسه محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int ProductId { get; set; }

        [Display(Name = "نام", Prompt = "مثلا علی رضایی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Name { get; set; }

        [Display(Name = "ایمیل", Prompt = "you@example.com")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        [EmailAddress(ErrorMessage = Messages.InvalidEmailError)]
        public string Email { get; set; }

        [Display(Name = "متن دیدگاه", Prompt = "نظر شما درباره این محصول...")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        [MinLength(10, ErrorMessage = Messages.MinLengthError)]
        public string Review { get; set; }

        [Display(Name = "امتیاز شما به این محصول", Prompt = "امتیاز شما به این محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 5, ErrorMessage = Messages.RangeError)]
        public int Stars { get; set; }
    }
}
