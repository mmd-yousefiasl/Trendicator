namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideBlogCommentDto
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public string? Reply { get; set; }
    }
}
