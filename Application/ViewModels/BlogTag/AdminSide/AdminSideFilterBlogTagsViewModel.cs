using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.BlogTag.AdminSide
{
    public class AdminSideFilterBlogTagsViewModel : BasePaging<AdminSideBlogTagViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
