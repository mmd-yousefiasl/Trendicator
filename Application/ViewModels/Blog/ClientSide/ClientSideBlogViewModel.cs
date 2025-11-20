namespace Application.ViewModels.Blog.ClientSide
{
    public class ClientSideBlogViewModel : CommonViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Writer { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
    }
}
