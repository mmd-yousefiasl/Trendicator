using Domain.Models;

namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideFilterBlogsViewModel : BasePaging<ClientSideBlogViewModel>
    {
        public string? CategorySlug { get; set; }
        public string? CategoryTitle { get; set; }
        public string? CategoryMetaDescription { get; set; }
    }
}
