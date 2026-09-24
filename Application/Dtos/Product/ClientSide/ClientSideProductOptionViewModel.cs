namespace Application.Dtos.Product.ClientSide
{
    public class ClientSideProductOptionViewModel : CommonDto
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal? OrderPrice { get; set; }
    }
}
