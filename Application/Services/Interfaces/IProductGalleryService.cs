using Application.ViewModels.ProductGallery.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductGalleryService
    {
        Task<Result<List<AdminSideProductGalleryViewModel>>> AdminSideGetProductGalleries(int id);

        Task<Result> CreateNewProductGallery(AdminSideCreateNewProductGalleryViewModel model);

        Task<Result> DeleteProductGallery(int id);
    }
}
