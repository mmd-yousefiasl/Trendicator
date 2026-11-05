using Domain.Models.Products;

namespace Domain.Models.Shopping
{
    public class CartItem
    {
        public string Title { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal DisplayPrice { get; set; }

        #region Relations
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
