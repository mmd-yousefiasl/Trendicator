namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartSummaryViewModel
    {
        public decimal SubTotal { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal SubTotalAfterDisocunt { get; set; }
        public decimal? Tax { get; set; }
        public decimal Total { get; set; }
        public int? TaxRate { get; set; }
        public string? DiscountCode { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountCodeAmount { get; set; }
    }
}
