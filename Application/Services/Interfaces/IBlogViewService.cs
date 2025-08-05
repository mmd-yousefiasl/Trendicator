using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBlogViewService
    {
        Task<Result> CreateNewBlogView(int id);
    }
}
