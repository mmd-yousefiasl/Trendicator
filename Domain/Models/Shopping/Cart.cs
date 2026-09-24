using Domain.Enums.Shopping;

namespace Domain.Models.Shopping
{
    public class Cart : AuditBaseEntity<int>
    {
        public string UserIP { get; set; }
        public int? UserId { get; set; }
        public CartStatusEnum Status { get; set; }
        public bool IsPaid { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Tax { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? TradingViewAccountName { get; set; }

        #region Relations

        public ICollection<CartItem>? Items { get; set; }

        #endregion
    }
}
