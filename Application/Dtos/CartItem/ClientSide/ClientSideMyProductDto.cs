using Domain.Enums.Products;
using Domain.Enums.Shopping;

namespace Application.Dtos.CartItem.ClientSide
{
    public class ClientSideMyProductDto
    {
        public int CartId { get; set; }
        public string ProductTitle { get; set; }
        public int ProductId { get; set; }
        public DateTime PaidDate { get; set; }
        public ProductTypeEnum Type { get; set; }
        public CartItemStatus Status { get; set; }
        public string? TradingViewAccountName { get; set; }
    }
}
