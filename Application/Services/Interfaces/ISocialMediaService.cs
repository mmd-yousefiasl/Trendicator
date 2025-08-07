using Application.ViewModels.SocialMedia.AdminSide;
using Application.ViewModels.SocialMedia.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISocialMediaService
    {
        Task<Result<AdminSideFilterSocialMediaViewModel>> FilterSocialMedia(AdminSideFilterSocialMediaViewModel filter);

        Task<Result> CreateNewSocialMedia(AdminSideCreateNewSocialMediaViewModel model);

        Task<Result<AdminSideUpdateSocialMediaViewModel>> GetSocialMediaForUpdate(int id);

        Task<Result> UpdateSocialMedia(AdminSideUpdateSocialMediaViewModel model);

        Task<Result> DeleteSocialMedia(int id);

        Task<Result<List<ClientSideSocialMediaViewModel>>> GetSocialMedia();
    }
}
