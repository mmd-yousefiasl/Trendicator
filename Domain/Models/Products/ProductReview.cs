using Domain.Enums.Products;

namespace Domain.Models.Products
{
    public class ProductReview : AuditBaseEntity<int>
    {
        public string Review { get; set; }
        public ProductReviewStarsEnum Stars { get; set; }
        public bool IsConfirmed { get; set; }

        #region Relations
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
