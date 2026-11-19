using Domain.Enums.Products;
using Domain.Enums.Shopping;
using Domain.Models.Products;

namespace Domain.Models.Shopping
{
    public class CartItem
    {
        public string Title { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal DisplayPrice { get; set; }
        public ProductTypeEnum ProductType { get; set; }
        public string? TradingViewAccountName { get; set; }
        public CartItemStatus Status { get; set; }

        #region Relations
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
