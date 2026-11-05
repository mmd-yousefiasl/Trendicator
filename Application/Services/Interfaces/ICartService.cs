using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Dtos.Payment;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result<ClientSideCartSnapshotDto>> ClientSideGetCartSnapshot();

        Task<Result<int>> ClientSideCreateNewCart();

        Task<List<int>> ClientSideGetCartProductIds();

        Task<int> ClientSideGetCartItemsCount();

        Task<Result<ClientSideCartDto>> ClientSideGetCart();

        Task<Result<int>> ClientSideAddToCart(ClientSideCartItemToggleDto model);

        Task<Result<int>> ClientSideRemoveFromCart(ClientSideCartItemToggleDto model);

        Task<Result> ClientSideUpdateCartAfterUserLoggedIn(int userId);

        Task<Result> ClientSideApplyCoupon(ClientApplyCouponDto model);

        Task<Result> ClientSideRemoveCoupon();

        Task<Result> ClientSideCheckout(PaymentResultViewModel model);

        Task<Result<ClientSidePaymentResultDto>> ClientSideVerifyCart();

        Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary();

        Task<Result> ClientSideAddUserIdToCurrentUserCart();

        Task<Result<string>> ClientSideGetCurrentUserTradingViewAccountName();

        Task<Result> ClientSideUpdateCurrentUserTradingViewAccountName(ClientSideUpdateTradingViewAccountNameViewModel model);

        Task<Result> CheckUserCanProccedToCheckout();

        Task<Result<ClientSideProceedToCheckoutViewModel>> ClientSideProceedToCheckout(bool useWallet);

        Task<Result<AdminSideFilterCartsViewModel>> AdminSideFilterCarts(AdminSideFilterCartsViewModel filter);

        Task<Result<AdminSideUpdateCartStatusViewModel>> AdminSideGetStatusForUpdate(int id);

        Task<Result> AdminSideUpdateCartStatus(AdminSideUpdateCartStatusViewModel model);

        Task<Result<AdminSideCartDetailsViewModel>> AdminSideGetCartDetails(int id);

        Task<Result<ClientSideFilterCartsViewModel>> ClientSideFilterCarts(ClientSideFilterCartsViewModel filter);

        Task<Result<ClientSideCartDetailsViewModel>> ClientSideGetCartDetails(int id);

        Task<int> OrdersCount();

        Task<decimal> OrdersValue();

        Task<int> MyOrdersCount();

        Task<decimal> MyTotalPurchesAmount();
    }
}
