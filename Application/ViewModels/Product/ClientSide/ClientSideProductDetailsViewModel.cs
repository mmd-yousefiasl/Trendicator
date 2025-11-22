using Application.ViewModels.ProductFaq.ClientSide;

namespace Application.ViewModels.Product.ClientSide
{
    public class ClientSideProductDetailsViewModel : BaseDiscountViewModel
    {
        public string BannerFileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public string CategorySlug { get; set; }
        public int CategoryId { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsInUserWishList { get; set; }
        public bool IsInUserCompare { get; set; }

        public List<ClientSideProductOptionViewModel>? Options { get; set; }
        public List<ClientSideProductFaqViewModel>? Faqs { get; set; }
    }
}
