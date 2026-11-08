using Domain.Enums.Shopping;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideOrderDto : CommonDto
    {
        public CartStatusEnum Status { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? TotalDiscount { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
