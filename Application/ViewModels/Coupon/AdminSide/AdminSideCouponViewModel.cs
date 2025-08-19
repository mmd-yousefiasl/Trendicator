namespace Application.ViewModels.Coupon.AdminSide
{
    public class AdminSideCouponViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Code { get; set; }
        public int Percent { get; set; }
    }
}
