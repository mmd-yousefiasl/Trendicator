namespace Domain.Models.Products
{
    public class ProductCategory : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Slug { get; set; }

        #region Realtions
        public ICollection<Product>? Products { get; set; }
        public int? ParentCategoryId { get; set; }
        public ProductCategory? ParentCategory { get; set; }
        public ICollection<ProductCategory>? SubCategories { get; set; }
        #endregion
    }
}
