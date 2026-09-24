using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Coupon.AdminSide
{
    public class AdminSideFilterCouponsViewModel : BasePaging<AdminSideCouponViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
