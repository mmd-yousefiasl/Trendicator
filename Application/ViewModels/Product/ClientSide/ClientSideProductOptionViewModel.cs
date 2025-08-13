namespace Application.ViewModels.Product.ClientSide
{
    public class ClientSideProductOptionViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }
    }
}
