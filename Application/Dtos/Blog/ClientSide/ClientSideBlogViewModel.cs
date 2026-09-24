using Application.Dtos;

namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideBlogViewModel : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Writer { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CategorySlug { get; set; }
    }
}
