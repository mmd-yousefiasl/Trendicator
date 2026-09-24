using Application.Dtos.SmsData.SmsConsumer;
using Application.Services.Interfaces;
using IPE.SmsIrClient;
using IPE.SmsIrClient.Models.Requests;

namespace Web.BackgroundServices
{
    public class SmsConsumerWorker : BackgroundService
    {
        private readonly ISmsQueue _queue;
        private readonly ILogger<SmsConsumerWorker> _logger;

        public SmsConsumerWorker(
            ISmsQueue queue,
            ILogger<SmsConsumerWorker> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Consumer Worker started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    SmsQueueItem smsItem = await _queue.DequeueSmsAsync(stoppingToken);

                    var verifySendParameters = smsItem.Parameters
                        .Select(p => new VerifySendParameter(p.Name, p.Value))
                        .ToArray();

                    var smsClient = new SmsIr(smsItem.ApiKey);

                    var result = await smsClient.VerifySendAsync(
                        smsItem.Mobile,
                        smsItem.PatternId,
                        verifySendParameters
                    );

                    if (result.Status != 1)
                    {
                        _logger.LogWarning("SMS sent but sms.ir returned status: {Status}", result.Status);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing queued SMS notification.");
                }
            }
        }
    }
}
