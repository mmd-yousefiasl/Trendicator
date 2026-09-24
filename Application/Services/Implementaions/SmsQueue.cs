using Application.Dtos.SmsData.SmsConsumer;
using Application.Services.Interfaces;
using System.Threading.Channels;

namespace Application.Services.Implementaions
{
    public class SmsQueue : ISmsQueue
    {
        #region Fields
        private readonly Channel<SmsQueueItem> _channel;
        #endregion

        #region Ctor
        public SmsQueue()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateUnbounded<SmsQueueItem>(options);
        }
        #endregion

        public ValueTask QueueSmsAsync(SmsQueueItem message)
        {
            return _channel.Writer.WriteAsync(message);
        }

        public ValueTask<SmsQueueItem> DequeueSmsAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
