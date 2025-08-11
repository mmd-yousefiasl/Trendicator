using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductCategory.AdminSide
{
    public class AdminSideFilterProductCategoriesViewMdoel : BasePaging<AdminSideProductCategoryViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
