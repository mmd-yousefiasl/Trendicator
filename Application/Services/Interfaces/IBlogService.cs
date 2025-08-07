using Application.ViewModels.Blog.AdminSide;
using Application.ViewModels.Blog.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBlogService
    {
        Task<Result<AdminSideFilterBlogsViewModel>> AdminSideFilterBlogs(AdminSideFilterBlogsViewModel filter);

        Task<Result> CreateNewBlog(AdminSideCreateNewBlogViewModel model);

        Task<Result<AdminSideUpdateBlogViewModel>> GetBlogForUpdate(int id);

        Task<Result> UpdateBlog(AdminSideUpdateBlogViewModel model);

        Task<Result> SoftDeleteBlog(int id);

        Task<Result<ClientSideFilterBlogsViewModel>> ClientSideFilterBlogs(ClientSideFilterBlogsViewModel filter);

        Task<Result<ClientSideBlogDetailsViewModel>> ClientSideGetBlogDetails(string slug);

        Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetLatestBlogs();

        Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetRelatedBlogs(int id, int categoryId);
    }
}
