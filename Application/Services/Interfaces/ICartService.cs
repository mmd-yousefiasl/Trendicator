using Application.ViewModels.Cart.ClientSide;
using Application.ViewModels.Payment;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result> AddToCart(int optionId);

        Task<Result<List<ClientSideCartViewModel>>> ClientSideGetCart();

        Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary();

        Task<Result> AddCoupon(ClientSideAddCouponToCartViewModel model);

        Task<Result> ClientSideAddUserIdToCurrentUserCart();

        Task<Result<string>> ClientSideGetCurrentUserTradingViewAccountName();

        Task<Result> ClientSideUpdateCurrentUserTradingViewAccountName(ClientSideUpdateTradingViewAccountNameViewModel model);

        Task<Result> CheckUserCanProccedToCheckout();

        Task<Result<ClientSideProceedToCheckoutViewModel>> ClientSideProceedToCheckout(bool useWallet);

        Task<Result> ClientSideCheckout(PaymentResultViewModel model);

        Task<Result<int>> ClientSideVerifyCart();
    }
}
