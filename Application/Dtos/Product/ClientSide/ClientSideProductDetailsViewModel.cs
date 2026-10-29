using Application.Dtos.ProductFaq.ClientSide;

namespace Application.Dtos.Product.ClientSide
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
        public string? ParentCategory { get; set; }
        public decimal ReviewRate { get; set; }
        public int ViewsCount { get; set; }
        public int ReviewsCount { get; set; }

        public List<ClientSideProductFaqViewModel>? Faqs { get; set; }
    }
}
