namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartDetailsDto : CommonDto
    {
        public DateTime PaidDate { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }

        public ICollection<ClientSIdeOrderItemDto>? Products { get; set; }
    }
}
