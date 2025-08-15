using Application.ViewModels.ProductWishList.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductWishListService
    {
        Task<bool> IsInUserWishList(int productId);

        Task<Result> AddProductToWishList(int productId);

        Task<Result> RemoveProductFromWishList(int productId);

        Task<Result<ClientSideFilterProductWishListViewModel>> ClientSideFilterWishList(ClientSideFilterProductWishListViewModel filter);

        Task<Result<List<int>>> GetCurrentUserWishListIds();

        Task<Result> DeleteWishList(int id);
    }
}
