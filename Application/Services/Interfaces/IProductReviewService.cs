using Application.ViewModels.ProductReview.AdminSide;
using Application.ViewModels.ProductReview.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductReviewService
    {
        Task<Result<AdminSideFilterProductReviewsViewModel>> AdminSideFilterProductReviews(AdminSideFilterProductReviewsViewModel filter);

        Task<Result> ConfirmReview(int id);

        Task<Result> RejectReview(int id);

        Task<Result> SoftDeleteProductReview(int id);

        Task<Result> ClientSideWriteProductReview(ClientSideWriteProductReviewViewModel model);

        Task<Result<ClientSideProductReviewRateDataViewModel>> GetProductReviewRate(int productId);

        Task<Result<List<ClientSideProductReviewRateDataViewModel>>> GetProductsReviewRate(List<int> productIds);

        Task<Result<List<ClientSideProductReviewViewModel>>> ClientSideProductReviews(int productId);
    }
}
