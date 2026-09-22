using Domain.Enums.Products;

namespace Application.ViewModels.ProductReview.AdminSide
{
    public class AdminSideProductReviewViewModel : CommonDto
    {
        public string Review { get; set; }
        public ProductReviewStarsEnum Stars { get; set; }
        public bool IsConfirmed { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
    }
}
