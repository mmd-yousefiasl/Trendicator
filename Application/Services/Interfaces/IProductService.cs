using Application.ViewModels.Product.AdminSide;
using Application.ViewModels.Product.ClientSide;
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

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetLatestProducts();

        Task<Result<ClientSideFilterProductsViewModel>> ClientSideFilterProducts(ClientSideFilterProductsViewModel filter);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetTopRatedProducts();

        Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(int id);

        Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(string slug);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetRelatedProducts(int id, int categoryId);

        Task<Result<int>> ClientSideGetProductIdByOptionId(int optionId);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetDiscountedProducts();

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostViewedProducts();

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostPurchasedProducts();
    }
}
