using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Coupon.AdminSide
{
    public class AdminSideUpdateCouponViewModel : CommonViewModel
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public DateTime FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public DateTime ToDate { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Code { get; set; }

        [Display(Name = "درصد")]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int Percent { get; set; }

        [Display(Name = "کاربران")]
        public List<int>? UserIds { get; set; }
    }
}
