namespace Application.ViewModels.Product.AdminSide
{
    public class AdminSideProductViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public long DisplayPrice { get; set; }
        public long? DisplayOrderPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public int ViewsCount { get; set; }
    }
}
