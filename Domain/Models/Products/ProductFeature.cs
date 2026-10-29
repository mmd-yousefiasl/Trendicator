namespace Domain.Models.Products
{
    public class ProductFeature : BaseEntity<int>
    {
        public string Text { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
