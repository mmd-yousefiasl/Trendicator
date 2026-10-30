namespace Application.Dtos.ProductReview.ClientSide
{
    public class ClientSideProductReviewDto
    {
        public string Name { get; set; }
        public string Review { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Stars { get; set; }
    }
}
