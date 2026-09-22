using Application.Services.Interfaces;
using Application.ViewModels.SmsData.ClientSide;
using Application.ViewModels.SmsData.SmsConsumer;
using Domain.Enums.SiteInfo;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementaions
{
    public class SendSmsService : ISendSmsService
    {
        #region Fields
        private readonly ISmsPanelService _smsDataService;
        private readonly ILogger<ISendSmsService> _logger;
        private readonly ISmsQueue _smsQueue;
        #endregion

        #region Ctor
        public SendSmsService(ISmsPanelService smsDataService,
            ILogger<ISendSmsService> logger,
            ISmsQueue smsQueue)
        {
            _smsDataService = smsDataService;
            _logger = logger;
            _smsQueue = smsQueue;
        }
        #endregion

        public async Task<Result> ClientSideSendLoginCode(ClientSideSendLoginCodeDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

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
