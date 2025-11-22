using Domain.Enums.Products;
using Domain.Models.Shopping;

namespace Domain.Models.Products
{
    public class Product : AuditBaseEntity<int>
    {
        public string BannerFileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public long DisplayPrice { get; set; }
        public long? DisplayOrderPrice { get; set; }
        public bool IsAvailable { get; set; }
        public ProductTypeEnum Type { get; set; }
        public string? MetaTraderLink { get; set; }
        public string? IndicatorFileName { get; set; }

        #region Relations

        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ICollection<ProductReview>? Reviews { get; set; }
        public ICollection<ProductWishList>? WishList { get; set; }
        public ICollection<ProductCompare>? Compare { get; set; }
        public ICollection<ProductView>? Views { get; set; }
        public ICollection<ProductOption>? Options { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<ProductFaq>? Faqs { get; set; }
        #endregion
    }
}
