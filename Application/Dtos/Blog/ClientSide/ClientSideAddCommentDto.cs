using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideAddCommentDto
    {
        [Display(Name = "شناسه بلاگ")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int BlogId { get; set; }

        [Display(Name = "نام", Prompt = "مثلا علی رضایی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Name { get; set; }

        [Display(Name = "ایمیل", Prompt = "you@example.com")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        [EmailAddress(ErrorMessage = Messages.InvalidEmailError)]
        public string Email { get; set; }

        [Display(Name = "متن دیدگاه", Prompt = "نظر شما درباره این مقاله...")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        [MinLength(10, ErrorMessage = Messages.MinLengthError)]
        public string Comment { get; set; }
    }
}
