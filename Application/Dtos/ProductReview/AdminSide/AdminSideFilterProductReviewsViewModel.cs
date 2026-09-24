using Domain.Enums.Products;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductReview.AdminSide
{
    public class AdminSideFilterProductReviewsViewModel : BasePaging<AdminSideProductReviewViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "امتیاز")]
        public ProductReviewStarsEnum? Stars { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsConfirmed { get; set; }
    }
}
