using Domain.Enums.Shopping;

namespace Application.ViewModels.Cart.AdminSide
{
    public class AdminSideCartViewModel : CommonViewModel
    {
        public string UserIP { get; set; }
        public int? UserId { get; set; }
        public CartStatusEnum Status { get; set; }
        public long? TotalPaid { get; set; }
        public long? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
