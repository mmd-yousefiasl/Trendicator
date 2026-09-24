using Application.Dtos.SmsData.ClientSide;
using Application.Dtos.SmsData.SmsConsumer;
using Application.Services.Interfaces;
using Domain.Enums.SiteInfo;
using Domain.Shared;

namespace Application.Services.Implementaions
{
    public class SendSmsService : ISendSmsService
    {
        #region Fields

        private readonly ISmsPanelService _smsDataService;
        private readonly ISmsQueue _smsQueue;

        #endregion

        #region Ctor

        public SendSmsService(ISmsPanelService smsDataService,
            ISmsQueue smsQueue)
        {
            _smsDataService = smsDataService;
            _smsQueue = smsQueue;
        }

        #endregion

        public async Task<Result> ClientSideSendLoginCode(ClientSideSendLoginCodeDto dto)
        {
            var infoResult = await _smsDataService.ClientSideGetSmsPanel(SmsPanelTypeEnum.OTP);
            if (infoResult.IsFailure)
                return Result.Failure(Messages.OperationFailedError);

            var loginSms = new SmsQueueItem(
                Mobile: dto.Mobile,
                ApiKey: infoResult.Value.ApiKey,
                PatternId: infoResult.Value.PatternId,
                Parameters: new List<SmsParameterItem>
                {
                    new("LOGINCODE", dto.Code)
                }
            );

            Console.WriteLine(dto.Code);
            //await _smsQueue.QueueSmsAsync(loginSms);

            return Result.Success();
        }
    }
}