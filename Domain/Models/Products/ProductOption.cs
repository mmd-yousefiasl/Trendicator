namespace Domain.Models.Products
{
    public class ProductOption : BaseEntity<int>
    {
        public string Title { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
