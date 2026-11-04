namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartSnapshotDto
    {
        public int CartId { get; set; }
        public string? DiscountCode { get; set; }
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
