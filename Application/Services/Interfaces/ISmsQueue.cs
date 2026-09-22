using Application.ViewModels.SmsData.SmsConsumer;

namespace Application.Services.Interfaces
{
    public interface ISmsQueue
    {
        ValueTask QueueSmsAsync(SmsQueueItem message);
        ValueTask<SmsQueueItem> DequeueSmsAsync(CancellationToken cancellationToken);
    }
}
