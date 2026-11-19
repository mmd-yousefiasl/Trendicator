using Domain.Enums.Products;
using Domain.Enums.Shopping;

namespace Application.Dtos.CartItem.AdminSide
{
    public class AdminSideCartItemDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal DisplayPrice { get; set; }
        public string? TradingViewAccountName { get; set; }
        public CartItemStatus Status { get; set; }
        public ProductTypeEnum Type { get; set; }
    }
}
