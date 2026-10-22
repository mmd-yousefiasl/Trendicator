using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductCategory.AdminSide
{
    public class AdminSideFilterProductCategoriesViewMdoel : BasePaging<AdminSideProductCategoryViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
