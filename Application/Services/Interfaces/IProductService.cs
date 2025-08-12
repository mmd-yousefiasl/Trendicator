using Application.ViewModels.Product.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<Result<AdminSideFilterProductsViewModel>> AdminSideFilterProducts(AdminSideFilterProductsViewModel filter);

        Task<Result> AdminSideCreateNewProduct(AdminSideCreateNewProductViewModel model);

        Task<Result<AdminSideUpdateProductViewModel>> AdminSideGetProductForUpdate(int id);

        Task<Result> AdminSideUpdateProduct(AdminSideUpdateProductViewModel model);

        Task<Result> AdminSideDeleteProduct(int id);
    }
}
