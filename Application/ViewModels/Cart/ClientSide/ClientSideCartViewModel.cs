namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideCartViewModel : CommonViewModel
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string OptionTitle { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}
