using Application.Dtos.Cart.AdminSide;
using Application.Dtos.Cart.ClientSide;
using Application.Dtos.CartItem.ClientSide;
using Application.Dtos.Payment;
using Application.Dtos.UserOrderStats.ClientSide;
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

        Task<Result<AdminSideCartDetailsDto>> AdminSideGetCartDetails(int id);

        Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary();

        Task<Result<string>> ClientSideGetCurrentUserTradingViewAccountName();

        Task<Result<ClientSideProceedToCheckoutDto>> ClientSideProceedToCheckout(bool useWallet);

        Task<Result<AdminSideFilterCartsDto>> AdminSideFilterCarts(AdminSideFilterCartsDto filter);

        Task<Result<ClientSideFilterCartsDto>> ClientSideFilterCarts(ClientSideFilterCartsDto filter);

        Task<Result<ClientSideCartDetailsDto>> ClientSideGetCartDetails(int id);

        Task<int> OrdersCount();

        Task<decimal> OrdersValue();

        Task<Result<ClientSideFilterMyProductsDto>> ClientSideFilterPurchasedProducts(ClientSideFilterMyProductsDto filter);

        Task<Result<ClientSideDownloadFileDto>> ClientSideGetFileInfo(int cartId, int productId);

        Task<Result<string>> ClientSideGetMetaTraderLink(int cartId, int productId);

        #region User Order Stats
        Task<Result<ClientSideUserOrderStatsDto>> ClientSideGetUserOrderStats();
        Task<Result> UserOrderStatsNeedUpdate(int userId);
        #endregion
    }
}
