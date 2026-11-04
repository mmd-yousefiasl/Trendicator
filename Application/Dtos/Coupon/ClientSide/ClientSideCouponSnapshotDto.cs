namespace Application.Dtos.Coupon.ClientSide
{
    public class ClientSideCouponSnapshotDto : CommonDto
    {
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Code { get; set; }
        public int Percent { get; set; }
    }
}
