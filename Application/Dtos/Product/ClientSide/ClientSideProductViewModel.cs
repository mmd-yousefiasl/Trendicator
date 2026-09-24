using Application.Dtos.Product;

namespace Application.Dtos.Product.ClientSide
{
    public class ClientSideProductViewModel : BaseDiscountViewModel
    {
        public string BannerFileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int ReviewRate { get; set; }
        public string ShortDescription { get; set; }
        public bool IsInUserWishList { get; set; }
        public bool IsAvailable { get; set; }
    }
}
