using Application.Services.Interfaces;
using Domain.Shared;
using IPE.SmsIrClient;
using IPE.SmsIrClient.Models.Requests;

namespace Application.Services.Implementaions
{
    public class SendSmsService : ISendSmsService
    {
        #region Fields
        private readonly ISmsDataService _smsService;
        #endregion

        #region Ctor
        public SendSmsService(ISmsDataService smsService)
        {
            _smsService = smsService;
        }
        #endregion

        public async Task<Result> SendLoginCode(string code, string mobile)
        {
            try
            {
                var infoResult = await _smsService.AdminSideGetSmsData();
                if (infoResult.IsFailure)
                    return Result.Failure(Messages.OperationFailedError);

                var info = infoResult.Value;
                if (string.IsNullOrWhiteSpace(info.SmsPanelAPIKey) || info.SmsPatternIdForLoginCode < 1)
                    return Result.Failure(Messages.OperationFailedError);

                SmsIr _sms = new SmsIr(info.SmsPanelAPIKey);

                var verificationSendResult = await _sms.VerifySendAsync(
                    mobile,
                    info.SmsPatternIdForLoginCode.Value,
                    new VerifySendParameter[] { new("LOGINCODE", code) }
                );

                return Result.Success(Messages.SuccessfullyDone);
            }
            catch (Exception ex)
            {
                return Result.Failure(Messages.OperationFailedError);
            }
        }
    }
}
