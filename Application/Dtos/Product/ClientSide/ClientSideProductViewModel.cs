namespace Application.Dtos.Product.ClientSide
{
    public class ClientSideProductViewModel : BaseDiscountViewModel
    {
        public string BannerFileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public decimal ReviewRate { get; set; }
        public string ShortDescription { get; set; }
        public string? Category { get; set; }
        public string? CategorySlug { get; set; }
        public string? ParentCategory { get; set; }
        public string? ParentCategorySlug { get; set; }
        public bool IsAvailable { get; set; }
    }
}
