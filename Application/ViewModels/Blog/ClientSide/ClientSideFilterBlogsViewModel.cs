using Domain.Models;

namespace Application.ViewModels.Blog.ClientSide
{
    public class ClientSideFilterBlogsViewModel : BasePaging<ClientSideBlogViewModel>
    {
        public int? TagId { get; set; }
        public int? CategoryId { get; set; }
    }
}
