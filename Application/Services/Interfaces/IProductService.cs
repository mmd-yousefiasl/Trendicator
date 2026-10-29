using Application.Dtos.Product.AdminSide;
using Application.Dtos.Product.ClientSide;
using Application.Dtos.ProductDetails.AdminSide;
using Application.Dtos.ProductFeature.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        #region Product
        Task<Result<AdminSideFilterProductsViewModel>> AdminSideFilterProducts(AdminSideFilterProductsViewModel filter);

        Task<Result> AdminSideCreateNewProduct(AdminSideCreateNewProductViewModel model);

        Task<Result<AdminSideUpdateProductViewModel>> AdminSideGetProductForUpdate(int id);

        Task<Result> AdminSideUpdateProduct(AdminSideUpdateProductViewModel model);

        Task<Result> AdminSideDeleteProduct(int id);

        Task<Result<ClientSideFilterProductsViewModel>> ClientSideFilterProducts(ClientSideFilterProductsViewModel filter);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetTopRatedProducts();

        Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(int id);

        Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(string slug);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetRelatedProducts(int id, int categoryId);

        Task<Result<int>> ClientSideGetProductIdByOptionId(int optionId);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetDiscountedProducts();

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostViewedProducts();

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostPurchasedProducts();

        Task<Result<List<AdminSideProductForSiteMapViewModel>>> AdminSideGetProductIdsForSiteMap();

        Task<Result> ClientSideAddProductView(int productId);
        #endregion

        #region ProductFeature
        Task<Result> AdminSideCreateNewProductFeature(AdminSideCreateNewProductFeatureDto model);

        Task<Result> AdminSideDeleteProductFeature(int id);

        Task<Result<AdminSideUpdateProductFeatureDto>> AdminSideGetProductFeatureForUpdate(int id);

        Task<Result> AdminSideUpdateProductFeature(AdminSideUpdateProductFeatureDto model);

        Task<Result<AdminSideFilterProductFeaturesDto>> AdminSideFilterProductFeatures(AdminSideFilterProductFeaturesDto filter);
        #endregion

        #region ProductDetails
        Task<Result> AdminSideCreateNewProductDeatils(AdminSideCreateNewProductDetailsDto model);

        Task<Result> AdminSideDeleteProductDetails(int id);

        Task<Result<AdminSideFilterProductDetailsDto>> AdminSideFilterProductDetails(AdminSideFilterProductDetailsDto filter);

        Task<Result<AdminSideUpdateProductDetailsDto>> AdminSideGetProductDetailsForUpdate(int id);

        Task<Result> AdminSideUpdateProductDetails(AdminSideUpdateProductDetailsDto model);
        #endregion
    }
}
