using Domain.Models;

namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideFilterBlogsViewModel : BasePaging<ClientSideBlogViewModel>
    {
        public int? TagId { get; set; }
        public string? Category { get; set; }
    }
}
