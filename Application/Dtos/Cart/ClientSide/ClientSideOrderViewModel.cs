using Domain.Enums.Shopping;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideOrderViewModel : CommonDto
    {
        public CartStatusEnum Status { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
