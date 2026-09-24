using Application.Dtos;

namespace Application.Dtos.ProductWishList.ClientSide
{
    public class ClientSideProductWishListViewModel : CommonDto
    {
        public int ProductId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
