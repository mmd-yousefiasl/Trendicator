using Application.Dtos.CartItem.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartItemService
    {

        Task<Result> IsAnyTradingViewProductInCart();

        Task<Result<ClientSideFilterBoughtFilesViewModel>> ClientSideFilterBoughtFiles(ClientSideFilterBoughtFilesViewModel filter);

        Task<Result<ClientSideDownloadFileViewModel>> CanUserDownlaodTheCurrentFile(int id);

        Task<int> SoldProductsCount();
    }
}
