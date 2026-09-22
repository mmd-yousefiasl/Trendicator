namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideCartDetailsViewModel : CommonDto
    {
        public DateTime PaidDate { get; set; }
        public long SubTotal { get; set; }
        public long Tax { get; set; }
        public long TotalPaid { get; set; }
        public long TotalDiscount { get; set; }
        public string? TradingViewAccountName { get; set; }

        public ICollection<ClientSIdeCartItemViewModel> Products { get; set; }
    }
}
