namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartSnapshotDto
    {
        public int CartId { get; set; }
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
