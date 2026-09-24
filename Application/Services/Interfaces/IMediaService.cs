using Application.Dtos.Media;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IMediaService
    {
        Task<Result<AdminSideFilterMediaViewModel>> AdminSideFilterMedia(AdminSideFilterMediaViewModel filter);

        Task<Result> AdminSideCreateNewMedia(AdminSideCreateNewMediaViewModel model);

        Task<Result> AdminSideDeleteMedia(int id);
    }
}
