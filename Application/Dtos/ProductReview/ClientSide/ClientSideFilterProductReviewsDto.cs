using Domain.Models;

namespace Application.Dtos.ProductReview.ClientSide
{
    public class ClientSideFilterProductReviewsDto : BasePaging<ClientSideProductReviewDto>
    {
        public int ProductId { get; set; }
        public bool HasMore { get; set; }
    }
}
