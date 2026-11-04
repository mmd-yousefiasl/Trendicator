using Application.Dtos;

namespace Application.Dtos.Coupon.AdminSide
{
    public class AdminSideCouponDto : CommonDto
    {
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Code { get; set; }
        public int Percent { get; set; }
    }
}
