using Domain.Models.Products;

namespace Domain.Models.Shopping
{
    public class CartItem : BaseEntity<int>
    {
        public int StockOptionId { get; set; }

        #region Relations

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        #endregion
    }
}
