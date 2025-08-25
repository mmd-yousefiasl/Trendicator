using Application.ViewModels.CartItem.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<Result> ClientSideCreateNewCartItem(ClientSideCreateNewCartItemViewModel model);

        Task<Result> ClientSideDeleteItemFromCart(int id);

        Task<Result> CheckIfCartItemsIsAvailable();

        Task<Result> IsAnyTradingViewProductInCart();

        Task<Result<ClientSideFilterBoughtFilesViewModel>> ClientSideFilterBoughtFiles(ClientSideFilterBoughtFilesViewModel filter);

        Task<Result<ClientSideDownloadFileViewModel>> CanUserDownlaodTheCurrentFile(int id);
    }
}
