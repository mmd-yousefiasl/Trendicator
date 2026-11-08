namespace Application.Dtos.UserOrderStats.ClientSide
{
    public class ClientSideUserOrderStatsDto
    {
        public int OrdersCount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public int PurchasedProductsCount { get; set; }
    }
}
