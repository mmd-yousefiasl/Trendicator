using Application.Dtos;

namespace Application.Dtos.BlogCategory.AdminSide
{
    public class AdminSideBlogCategoryViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
