using Application.Dtos.ProductCompare.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductCompareService
    {
        Task<Result<ClientSideFilterProductComareViewModel>> ClientSideFilterProductCompare(ClientSideFilterProductComareViewModel filter);

        Task<Result> AddToCompare(int productId);

        Task<Result> RemoveFromCompare(int productId);

        Task<Result> DeleteCompare(int id);

        Task<bool> IsInUserCompare(int productId);

        Task<Result<List<int>>> GetCurrentUserCompare();
    }
}
