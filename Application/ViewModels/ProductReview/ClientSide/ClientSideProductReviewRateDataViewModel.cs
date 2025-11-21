namespace Application.ViewModels.ProductReview.ClientSide
{
    public class ClientSideProductReviewRateDataViewModel
    {
        public string ProductTitle { get; set; }
        public int ProductId { get; set; }
        public int TotalCount { get; set; }
        public decimal Avrage { get; set; }
        public int RoundedAvrage { get; set; }
    }
}
