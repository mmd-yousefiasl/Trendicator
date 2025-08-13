using Application.ViewModels.ProductOption.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductOptionService
    {
        Task<Result<AdminSideFilterProductOptionsViewModel>> AdminSideFilterProductOptions(AdminSideFilterProductOptionsViewModel filter);

        Task<Result> AdminSideCreateNewProductOption(AdminSideCreateNewProductOptionViewModel model);

        Task<Result<AdminSideUpdateProductOptionViewModel>> AdminSideGetProductOptionForUpdate(int id);

        Task<Result> AdminSideUpdateProductOption(AdminSideUpdateProductOptionViewModel model);

        Task<Result> AdminSideDeleteProductOption(int id);
    }
}
