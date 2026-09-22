namespace Application.ViewModels.ProductCategory.AdminSide
{
    public class AdminSideProductCategoryViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? ParentCategoryTitle { get; set; }
    }
}
