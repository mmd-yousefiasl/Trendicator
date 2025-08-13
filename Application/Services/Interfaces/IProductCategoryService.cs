using Application.ViewModels.ProductCategory.AdminSide;
using Application.ViewModels.ProductCategory.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<Result<AdminSideFilterProductCategoriesViewMdoel>> AdminSideFilterProductCategories(AdminSideFilterProductCategoriesViewMdoel filter);

        Task<Result> AdminSideCreateNewProductCategory(AdminSideCreateNewProductCategoryViewModel model);

        Task<Result<AdminSideUpdateProductCategoryViewModel>> AdminSideGetCategoryForUpdate(int id);

        Task<Result> AdminSideUpdateProductCategory(AdminSideUpdateProductCategoryViewModel model);

        Task<Result> AdminSideDeleteProductCategory(int id);

        Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetProductCategoriesAsOptions(int? id);

        Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetProductCategories();
    }
}
