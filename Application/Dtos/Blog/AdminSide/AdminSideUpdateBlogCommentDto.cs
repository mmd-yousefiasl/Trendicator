using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideUpdateBlogCommentDto : CommonDto
    {
        [Display(Name = "نظر")]
        public string? Comment { get; set; }

        public int BlogId { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "پاسخ")]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        public string? Reply { get; set; }
    }
}
