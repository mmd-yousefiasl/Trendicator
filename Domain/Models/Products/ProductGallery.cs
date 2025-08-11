namespace Domain.Models.Products
{
    public class ProductGallery : BaseEntity<int>
    {
        public string FileName { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
