namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCartViewModel : CommonDto
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string OptionTitle { get; set; }
        public decimal Price { get; set; }
        public decimal? OrderPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}
