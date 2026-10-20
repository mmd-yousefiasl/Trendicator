namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideBlogViewModel : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string MetaDescription { get; set; }
        public int TimeToRead { get; set; }
        public int ViewsCount { get; set; }
    }
}
