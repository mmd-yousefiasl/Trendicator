namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSidePaymentResultDto
    {
        public DateTime CurrentDate { get; set; } = DateTime.UtcNow;
        public int? CartId { get; set; }
    }
}
