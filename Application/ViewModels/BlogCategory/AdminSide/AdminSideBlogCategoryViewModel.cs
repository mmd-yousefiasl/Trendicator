namespace Application.ViewModels.BlogCategory.AdminSide
{
    public class AdminSideBlogCategoryViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
