namespace Domain.Models.Coupons
{
    public class Coupon : AuditBaseEntity<int>
    {
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Code { get; set; }
        public int Percent { get; set; }
    }
}
