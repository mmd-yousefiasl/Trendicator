namespace Application.ViewModels.Product.ClientSide
{
    public class ClientSideProductOptionViewModel : CommonDto
    {
        public string Title { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }
    }
}
