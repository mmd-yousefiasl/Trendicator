namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideBlogViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Writer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public string Category { get; set; }
        public int TimeToRead { get; set; }
    }
}
