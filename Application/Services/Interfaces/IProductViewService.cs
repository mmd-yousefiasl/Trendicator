using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductViewService
    {
        Task<Result> AddProductView(int productId);
    }
}
