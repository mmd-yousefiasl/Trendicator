using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.BlogCategory.AdminSide
{
    public class AdminSideFilterBlogCategoriesViewModel : BasePaging<AdminSideBlogCategoryViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
