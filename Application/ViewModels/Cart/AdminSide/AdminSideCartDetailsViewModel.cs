namespace Application.ViewModels.Cart.AdminSide
{
    public class AdminSideCartDetailsViewModel : CommonViewModel
    {
        public DateTime PaidDate { get; set; }
        public long SubTotal { get; set; }
        public long Tax { get; set; }
        public long TotalPaid { get; set; }
        public long TotalDiscount { get; set; }
        public string? TradingViewAccountName { get; set; }

        public ICollection<AdminSideCartItemViewModel> Products { get; set; }
    }
}
