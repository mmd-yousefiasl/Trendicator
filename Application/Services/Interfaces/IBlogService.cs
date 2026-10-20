using Application.Dtos.Blog.AdminSide;
using Application.Dtos.Blog.ClientSide;
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

        Task<Result<ClientSideBlogDetailsViewModel>> ClientSideGetBlogDetails(int id);

        Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetLatestBlogs();

        Task<Result<ClientSideFilterBlogsViewModel>> ClientSideGetRelatedBlogs(int id, int categoryId);

        Task<Result<List<AdminSideBlogForSiteMapViewModel>>> AdminSideGetBlogIdsForSiteMap();

        Task<Result> ClientSideAddBlogView(int blogId);

        Task<Result> ClientSideAddNewBlogComment(ClientSideAddCommentDto model);

        Task<Result<AdminSideFilterBlogCommentsDto>> AdminSideFilterComments(AdminSideFilterBlogCommentsDto filter);

        Task<Result<int>> AdminSideDeleteBlogComment(int id);

        Task<Result<AdminSideUpdateBlogCommentDto>> AdminSideGetBlogCommentForUpdate(int id);

        Task<Result<int>> AdminSideUpdateBlogComment(AdminSideUpdateBlogCommentDto model);
    }
}
