namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartDto : CommonDto
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Hint { get; set; }
        public decimal DisplayPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}
