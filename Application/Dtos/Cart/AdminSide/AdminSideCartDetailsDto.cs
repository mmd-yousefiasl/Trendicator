using Application.Dtos.CartItem.AdminSide;

namespace Application.Dtos.Cart.AdminSide
{
    public class AdminSideCartDetailsDto : CommonDto
    {
        public DateTime PaidDate { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public string DiscountCode { get; set; }

        public ICollection<AdminSideCartItemDto> Products { get; set; }
    }
}
