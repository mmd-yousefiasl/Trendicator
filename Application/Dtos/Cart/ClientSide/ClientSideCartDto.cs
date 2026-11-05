using Application.Dtos.CartItem.ClientSide;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartDto : CommonDto
    {
        public int ItemsCount { get; set; }
        public decimal DisplayPriceSum { get; set; }
        public decimal OrderPriceSum { get; set; }
        public decimal ProductsDiscount { get; set; }
        public string? DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountCodeDiscount { get; set; }
        public decimal PayAmount { get; set; }
        public List<ClientSideCartItemDto>? Items { get; set; }
        public bool HaveUnavailableProduct { get; set; }
    }
}
