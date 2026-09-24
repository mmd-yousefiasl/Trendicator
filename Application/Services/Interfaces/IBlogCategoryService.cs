using Application.Dtos.BlogCategory.AdminSide;
using Application.Dtos.BlogCategory.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBlogCategoryService
    {
        Task<Result<AdminSideFilterBlogCategoriesViewModel>> FilterBlogCategories(AdminSideFilterBlogCategoriesViewModel filter);

        Task<Result> CreateNewBlogCategory(AdminSideCreateNewBlogCategoryViewModel model);

        Task<Result<AdminSideUpdateBlogCategoryViewModel>> GetBlogCategoryForUpdate(int id);

        Task<Result> UpdateBlogCategory(AdminSideUpdateBlogCategoryViewModel model);

        Task<Result> SoftDeleteBlogCategory(int id);

        Task<Result<List<AdminSideBlogCategoryAsOptionViewModel>>> GetCategoriesAsOptions();

        Task<Result<List<ClientSideBlogCategoryViewModel>>> ClientSideGetBlogCategories();
    }
}
