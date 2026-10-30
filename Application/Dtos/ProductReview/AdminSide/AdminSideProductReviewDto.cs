namespace Application.Dtos.ProductReview.AdminSide
{
    public class AdminSideProductReviewDto : CommonDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Stars { get; set; }
        public string Review { get; set; }
        public bool IsPublished { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
    }
}
