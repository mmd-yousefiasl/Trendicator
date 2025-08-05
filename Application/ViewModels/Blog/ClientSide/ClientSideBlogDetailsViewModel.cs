namespace Application.ViewModels.Blog.ClientSide
{
    public class ClientSideBlogDetailsViewModel : CommonViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Writer { get; set; }
        public string Body { get; set; }
    }
}