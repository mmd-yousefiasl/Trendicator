using Domain.Models.Users;

namespace Domain.Models.Coupons
{
    public class CouponUserMapping
    {
        public int CouponId { get; set; }
        public int UserId { get; set; }

        #region Relations
        public User User { get; set; }
        public Coupon Coupon { get; set; }
        #endregion
    }
}
