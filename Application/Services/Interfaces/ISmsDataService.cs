using Application.ViewModels.SmsData.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISmsDataService
    {
        Task<Result<AdminSideUpdateSmsDataViewModel>> AdminSideGetSmsDataForUpdate();

        Task<Result> AdminSideUpdateSmsData(AdminSideUpdateSmsDataViewModel model);

        Task<Result<AdminSideSmsDataViewModel>> AdminSideGetSmsData();
    }
}
