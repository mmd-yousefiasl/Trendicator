using Application.Dtos.EmailData.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IEmailDataService
    {
        Task<Result<AdminSideEmailDataViewModel>> GetEmailData();

        Task<Result<AdminSideUpdateEmailDataViewModel>> GetEmailDataForUpdate();

        Task<Result> UpdateEmailData(AdminSideUpdateEmailDataViewModel model);
    }
}
