using Application.ViewModels.OurFeature.AdminSide;
using Application.ViewModels.OurFeature.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IOurFeaturesService
    {
        Task<Result<AdminSideFilterOurFeaturesViewModel>> FilterOurFeatures(AdminSideFilterOurFeaturesViewModel filter);

        Task<Result> CreateNewOurFeature(AdminSideCreateNewOurFeatureViewModel model);

        Task<Result<AdminSideUpdateOurFeatureViewModel>> GetOurFeatureForUpdate(int id);

        Task<Result> UpdateOurFeature(AdminSideUpdateOurFeatureViewModel model);

        Task<Result> SoftDeleteOurFeature(int id);

        Task<Result<List<ClientSideOurFeatureViewModel>>> GetOurFeatures();
    }
}
