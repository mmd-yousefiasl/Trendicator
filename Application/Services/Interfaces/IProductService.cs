using Application.Dtos.Product.AdminSide;
using Application.Dtos.Product.ClientSide;
using Application.Dtos.ProductCategory.AdminSide;
using Application.Dtos.ProductCategory.ClientSide;
using Application.Dtos.ProductDetails.AdminSide;
using Application.Dtos.ProductFaq.AdminSide;
using Application.Dtos.ProductFeature.AdminSide;
using Application.Dtos.ProductReview.AdminSide;
using Application.Dtos.ProductReview.ClientSide;
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

        Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(int id);

        Task<Result<List<ClientSideProductViewModel>>> ClientSideGetRelatedProducts(int id, int categoryId);

        Task<Result<int>> ClientSideGetProductIdByOptionId(int optionId);

        Task<Result<List<AdminSideProductForSiteMapViewModel>>> AdminSideGetProductIdsForSiteMap();

        Task<Result> ClientSideAddProductView(int productId);
        #endregion

        #region Product Category
        Task<Result<AdminSideFilterProductCategoriesViewMdoel>> AdminSideFilterProductCategories(AdminSideFilterProductCategoriesViewMdoel filter);

        Task<Result> AdminSideCreateNewProductCategory(AdminSideCreateNewProductCategoryViewModel model);

        Task<Result<AdminSideUpdateProductCategoryViewModel>> AdminSideGetCategoryForUpdate(int id);

        Task<Result> AdminSideUpdateProductCategory(AdminSideUpdateProductCategoryViewModel model);

        Task<Result> AdminSideDeleteProductCategory(int id);

        Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetProductCategoriesAsOptions(int? id);

        Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetProductCategories();

        Task<Result<List<AdminSideProductCategoryAsOptionViewModel>>> AdminSideGetCategoriesForSiteMap();

        Task<Result<List<ClientSideProductCategoryViewModel>>> ClientSideGetParentCategories();
        #endregion

        #region Product Feature
        Task<Result> AdminSideCreateNewProductFeature(AdminSideCreateNewProductFeatureDto model);

        Task<Result> AdminSideDeleteProductFeature(int id);

        Task<Result<AdminSideUpdateProductFeatureDto>> AdminSideGetProductFeatureForUpdate(int id);

        Task<Result> AdminSideUpdateProductFeature(AdminSideUpdateProductFeatureDto model);

        Task<Result<AdminSideFilterProductFeaturesDto>> AdminSideFilterProductFeatures(AdminSideFilterProductFeaturesDto filter);
        #endregion

        #region Product Details
        Task<Result> AdminSideCreateNewProductDeatils(AdminSideCreateNewProductDetailsDto model);

        Task<Result> AdminSideDeleteProductDetails(int id);

        Task<Result<AdminSideFilterProductDetailsDto>> AdminSideFilterProductDetails(AdminSideFilterProductDetailsDto filter);

        Task<Result<AdminSideUpdateProductDetailsDto>> AdminSideGetProductDetailsForUpdate(int id);

        Task<Result> AdminSideUpdateProductDetails(AdminSideUpdateProductDetailsDto model);
        #endregion

        #region Product Comment

        Task<Result> ClientSideAddProductComment(ClientSideAddProductCommentDto model);

        Task<Result<AdminSideFilterProductReviewsDto>> AdminSideFilterProductReviews(AdminSideFilterProductReviewsDto filter);

        Task<Result> AdminSideConfirmReview(int id);

        Task<Result> AdminSideRejectReview(int id);

        Task<Result> AdminSideSoftDeleteProductReview(int id);

        Task<Result<ClientSideFilterProductReviewsDto>> ClientSideFilterReviews(ClientSideFilterProductReviewsDto filter);
        #endregion

        #region Product Faq
        Task<Result<AdminSideFilterProductFaqsViewModel>> AdminSideFilterProductFaqs(AdminSideFilterProductFaqsViewModel filter);

        Task<Result> AdminSideCreateNewProductFaq(AdminSideCreateNewProductFaqViewModel model);

        Task<Result<AdminSideUpdateProductFaqViewModel>> AdminSideGetProductFaqForUpdate(int id);

        Task<Result> AdminSideUpdateProductFaq(AdminSideUpdateProductFaqViewModel model);

        Task<Result> AdminSideDeleteProductFaq(int id);
        #endregion
    }
}
