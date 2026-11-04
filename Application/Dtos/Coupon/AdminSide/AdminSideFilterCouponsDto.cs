using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Coupon.AdminSide
{
    public class AdminSideFilterCouponsDto : BasePaging<AdminSideCouponDto>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
