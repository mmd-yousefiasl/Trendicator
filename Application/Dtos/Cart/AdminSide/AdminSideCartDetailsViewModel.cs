namespace Application.Dtos.Cart.AdminSide
{
    public class AdminSideCartDetailsViewModel : CommonDto
    {
        public DateTime PaidDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public string? TradingViewAccountName { get; set; }

        public ICollection<AdminSideCartItemViewModel> Products { get; set; }
    }
}
