using Domain.Enums.Products;
using Domain.Models.Shopping;

namespace Domain.Models.Products
{
    public class Product : AuditBaseEntity<int>
    {
        public string BannerFileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Hint { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public decimal DisplayPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public bool IsAvailable { get; set; }
        public ProductTypeEnum Type { get; set; }
        public string? MetaTraderLink { get; set; }
        public string? IndicatorFileName { get; set; }
        public decimal ReviewRate { get; set; }
        public int ReviewsCount { get; set; }
        public int ViewsCount { get; set; }
        public int PurchaseCount { get; set; }

        #region Relations

        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ICollection<ProductReview>? Reviews { get; set; }
        public ICollection<ProductView>? Views { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<ProductFaq>? Faqs { get; set; }
        public ICollection<ProductFeature>? Features { get; set; }
        #endregion
    }
}
