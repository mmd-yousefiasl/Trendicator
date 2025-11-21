using Application.ViewModels.BlogTag.ClientSide;

namespace Application.ViewModels.Blog.ClientSide
{
    public class ClientSideBlogDetailsViewModel : CommonViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Writer { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CategorySlug { get; set; }
        public List<ClientSideBlogTagViewModel>? Tags { get; set; }
    }
}