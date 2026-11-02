using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.Payment;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<int> ClientSideGetCartItemsCount();

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
