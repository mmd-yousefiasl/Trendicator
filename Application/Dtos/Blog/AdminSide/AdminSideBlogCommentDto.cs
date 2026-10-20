namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideBlogCommentDto : CommonDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public bool IsPublished { get; set; }
        public bool IsAnswered { get; set; }
    }
}
