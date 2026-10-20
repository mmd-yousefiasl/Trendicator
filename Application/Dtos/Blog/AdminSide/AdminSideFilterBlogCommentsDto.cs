using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideFilterBlogCommentsDto : BasePaging<AdminSideBlogCommentDto>
    {
        public int BlogId { get; set; }

        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }

        [Display(Name = "وضعیت")]
        public bool? IsAnswered { get; set; }
    }
}
