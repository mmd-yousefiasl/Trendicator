namespace Domain.Models.Shopping
{
    public class UserOrderStats
    {
        public int UserId { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public int PurchasedProductsCount { get; set; }
        public bool NeedToUpdate { get; set; } = false;
    }
}
