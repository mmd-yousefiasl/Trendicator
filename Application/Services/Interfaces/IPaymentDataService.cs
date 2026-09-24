using Application.Dtos.PaymentData;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IPaymentDataService
    {
        Task<Result<AdminSideUpdatePaymentDataViewModel>> AdminSideGetPaymentDataForUpdate();

        Task<Result> AdminSideUpdatePaymentData(AdminSideUpdatePaymentDataViewModel model);

        Task<Result<int>> GetTaxRate();

        Task<Result<string>> GetPin();
    }
}
