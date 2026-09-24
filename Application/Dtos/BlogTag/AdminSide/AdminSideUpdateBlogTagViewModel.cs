using Application.Dtos;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.BlogTag.AdminSide
{
    public class AdminSideUpdateBlogTagViewModel : CommonDto
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }
    }
}
