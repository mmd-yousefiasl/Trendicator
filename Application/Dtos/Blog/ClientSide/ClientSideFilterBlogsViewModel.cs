using Domain.Models;

namespace Application.Dtos.Blog.ClientSide
{
    public class ClientSideFilterBlogsViewModel : BasePaging<ClientSideBlogViewModel>
    {
        public string? Category { get; set; }
    }
}
