namespace Application.ViewModels.ProductWishList.ClientSide
{
    public class ClientSideProductWishListViewModel : CommonViewModel
    {
        public int ProductId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
