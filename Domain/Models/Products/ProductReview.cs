using Domain.Enums.Products;

namespace Domain.Models.Products
{
    public class ProductReview : AuditBaseEntity<int>
    {
        public string UserName { get; set; }
        public string Review { get; set; }
        public ProductReviewStarsEnum Stars { get; set; }
        public bool IsConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }

        #region Relations

        public int ProductId { get; set; }
        public Product Product { get; set; }

        #endregion
    }
}
