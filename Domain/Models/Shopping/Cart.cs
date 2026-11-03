using Domain.Enums.Shopping;

namespace Domain.Models.Shopping
{
    public class Cart : AuditBaseEntity<int>
    {
        public CartStatusEnum Status { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? UserId { get; set; }
        public string? VisitorId { get; set; }

        #region Relations
        public ICollection<CartItem>? Items { get; set; }
        #endregion
    }
}
