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
        public int CategoryId { get; set; }

        public List<ClientSideProductOptionViewModel>? Options { get; set; }
        public List<string>? Gallery { get; set; }
    }
}
