using Application.Services.Interfaces;
using Application.ViewModels.Payment;
using Domain.Shared;
using Domain.Statics;
using System.Net.Http.Json;

namespace Application.Services.Implementaions
{
    public class PaymentService : IPaymentService
    {
        #region Fields
        private readonly IPaymentDataService _paymentDataService;
        #endregion

        #region Ctor
        public PaymentService(IPaymentDataService paymentDataService)
        {
            _paymentDataService = paymentDataService;
        }
        #endregion

        public async Task<Result<PaymentRequestResultViewModel>> Request(PaymentRequestViewModel model)
        {
            if (model is null) return Result.Failure<PaymentRequestResultViewModel>(Messages.OperationFailedError);

            var pinResult = await _paymentDataService.GetPin();
            if (pinResult.IsFailure) return Result.Failure<PaymentRequestResultViewModel>(pinResult.Message);

            var requestData = new
            {
                merchant = pinResult.Value,
                amount = model.Amount,
                callbackUrl = model.CallBackUrl
            };

            using var client = new HttpClient();

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(SiteTools.PaymentRequestURL, requestData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PaymentRequestResultViewModel>();
                    return result;
                }
                else
                {
                    return Result.Failure<PaymentRequestResultViewModel>(Messages.OperationFailedError);
                }
            }
            catch
            {
                return Result.Failure<PaymentRequestResultViewModel>(Messages.OperationFailedError);
            }
        }

        public async Task<Result<VerifyPaymentResultViewModel>> VerifyPayment(VerifyPaymentViewModel model)
        {
            if (model is null) return Result.Failure<VerifyPaymentResultViewModel>(Messages.OperationFailedError);

            var pinResult = await _paymentDataService.GetPin();
            if (pinResult.IsFailure) return Result.Failure<VerifyPaymentResultViewModel>(pinResult.Message);

            var requestData = new
            {
                merchant = pinResult.Value,
                trackId = model.TrackId
            };

            using var client = new HttpClient();

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(SiteTools.VerifyPaymentURL, requestData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<VerifyPaymentResultViewModel>();
                    return result;
                }
                else
                {
                    return Result.Failure<VerifyPaymentResultViewModel>(Messages.OperationFailedError);
                }
            }
            catch
            {
                return Result.Failure<VerifyPaymentResultViewModel>(Messages.OperationFailedError);
            }
        }
    }
}
