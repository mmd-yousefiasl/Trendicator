using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductReview.AdminSide
{
    public class AdminSideFilterProductReviewsDto : BasePaging<AdminSideProductReviewDto>
    {
        [Display(Name = "امتیاز")]
        public int? Stars { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
