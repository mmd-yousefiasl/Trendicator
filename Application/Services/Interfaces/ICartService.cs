using Application.ViewModels.Cart.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result> AddToCart(int optionId);

        Task<Result<List<ClientSideCartViewModel>>> ClientSideGetCart();

        Task<Result<ClientSideCartSummaryViewModel>> ClientSideCartSummary();
    }
}
