namespace Domain.Models.Products
{
    public class ProductDetails : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
