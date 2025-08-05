using Application.ViewModels.BlogTag.AdminSide;
using Application.ViewModels.BlogTag.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBlogTagService
    {
        Task<Result<AdminSideFilterBlogTagsViewModel>> FilterBlogTags(AdminSideFilterBlogTagsViewModel filter);

        Task<Result> CreateNewBlogTag(AdminSideCreateNewBlogTagViewModel model);

        Task<Result<AdminSideUpdateBlogTagViewModel>> GetBlogTagForUpdate(int id);

        Task<Result> UpdateBlogTag(AdminSideUpdateBlogTagViewModel model);

        Task<Result> SoftDeleteBlogTag(int id);

        Task<Result<List<AdminSideBlogTagAsOptionViewModel>>> GetTagsAsOptions();

        Task<Result<List<ClientSideBlogTagViewModel>>> ClientSideGetBlogTags();
    }
}
