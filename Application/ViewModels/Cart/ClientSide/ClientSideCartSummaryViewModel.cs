namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideCartSummaryViewModel
    {
        public long SubTotal { get; set; }
        public long? TotalDiscount { get; set; }
        public long SubTotalAfterDisocunt { get; set; }
        public long? Tax { get; set; }
        public long Total { get; set; }
        public int? TaxRate { get; set; }
        public string? DiscountCode { get; set; }
        public int? DiscountPercent { get; set; }
        public long? DiscountCodeAmount { get; set; }
    }
}
