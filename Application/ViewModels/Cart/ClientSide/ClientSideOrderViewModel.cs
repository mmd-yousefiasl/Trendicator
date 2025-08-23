using Domain.Enums.Shopping;

namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideOrderViewModel : CommonViewModel
    {
        public CartStatusEnum Status { get; set; }
        public long? TotalPaid { get; set; }
        public long? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
