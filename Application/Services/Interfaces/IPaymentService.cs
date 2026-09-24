using Application.Dtos.Payment;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentRequestResultViewModel>> Request(PaymentRequestViewModel model);

        Task<Result<VerifyPaymentResultViewModel>> VerifyPayment(VerifyPaymentViewModel model);
    }
}
