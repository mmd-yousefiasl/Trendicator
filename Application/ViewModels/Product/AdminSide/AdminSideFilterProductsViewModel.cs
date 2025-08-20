using Domain.Enums.Products;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Product.AdminSide
{
    public class AdminSideFilterProductsViewModel : BasePaging<AdminSideProductViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }

        [Display(Name = "وضعیت موجودی")]
        public bool? IsAvailable { get; set; }

        [Display(Name = "نوع")]
        public ProductTypeEnum? Type { get; set; }
    }
}
