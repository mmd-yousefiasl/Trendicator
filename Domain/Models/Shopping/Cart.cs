using Domain.Enums.Shopping;

namespace Domain.Models.Shopping
{
    public class Cart : AuditBaseEntity<int>
    {
        public string UserIP { get; set; }
        public int? UserId { get; set; }
        public CartStatusEnum Status { get; set; }
        public bool IsPaid { get; set; }
        public long? TotalPaid { get; set; }
        public long? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public long? SubTotal { get; set; }
        public long? Tax { get; set; }
        public DateTime? PaidDate { get; set; }

        #region Relations

        public ICollection<CartItem>? Items { get; set; }

        #endregion
    }
}
