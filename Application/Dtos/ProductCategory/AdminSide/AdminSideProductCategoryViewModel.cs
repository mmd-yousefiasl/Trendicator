using Application.Dtos;

namespace Application.Dtos.ProductCategory.AdminSide
{
    public class AdminSideProductCategoryViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? ParentCategoryTitle { get; set; }
    }
}
