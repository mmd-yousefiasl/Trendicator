using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideFilterBlogsViewModel : BasePaging<AdminSideBlogViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
