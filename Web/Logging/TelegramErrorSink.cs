using Domain.Statics;
using Serilog.Core;
using Serilog.Events;
using System.Text;
using System.Threading.Channels;

namespace Web.Logging
{
    /// <summary>
    /// Serilog sink that forwards Error (and Fatal) log events to a private
    /// Telegram channel through the Cloudflare worker proxy.
    /// Events are queued and flushed as batches so an error storm can't flood
    /// the channel or hit Telegram's rate limits. The sink never throws and
    /// never logs — a failure here must not cause another error.
    /// </summary>
    public class TelegramErrorSink : ILogEventSink
    {
        private const int FlushIntervalSeconds = 10;

        // Telegram's hard message limit is 4096 chars — stay safely below it.
        private const int MaxBatchLength = 3500;
        private const int MaxEventLength = 1500;

        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private readonly Channel<string> _queue = Channel.CreateBounded<string>(
            new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            });

        public TelegramErrorSink()
        {
            _ = Task.Run(ProcessQueueAsync);
        }

        public void Emit(LogEvent logEvent)
        {
            if (!SiteTools.TelegramErrorAlertingEnabled ||
                SiteTools.TelegramErrorChannelId == 0 ||
                logEvent.Level < LogEventLevel.Error)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.Append('[')
                .Append(logEvent.Timestamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"))
                .Append("] ")
                .AppendLine(logEvent.RenderMessage());

            if (logEvent.Exception is not null)
            {
                builder.AppendLine(logEvent.Exception.ToString());
            }

            var text = builder.ToString();

            if (text.Length > MaxEventLength)
            {
                text = text[..MaxEventLength] + "\n…";
            }

            _queue.Writer.TryWrite(text);
        }

        private async Task ProcessQueueAsync()
        {
            while (await _queue.Reader.WaitToReadAsync())
            {
                // Wait briefly so bursts land in the same batch.
                await Task.Delay(TimeSpan.FromSeconds(FlushIntervalSeconds));

                var batch = new StringBuilder();

                while (_queue.Reader.TryRead(out var item))
                {
                    if (batch.Length > 0 &&
                        batch.Length + item.Length > MaxBatchLength)
                    {
                        await TrySendBatchAsync(batch.ToString());
                        batch.Clear();
                    }

                    batch.Append(item).AppendLine().AppendLine();
                }

                if (batch.Length > 0)
                {
                    await TrySendBatchAsync(batch.ToString());
                }
            }
        }

        private async Task TrySendBatchAsync(string batchText)
        {
            try
            {
                // Stack traces are full of angle brackets — escape everything and
                // render as code, otherwise Telegram rejects unbalanced HTML.
                var escaped = batchText
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");

                var payload = new
                {
                    chat_id = SiteTools.TelegramErrorChannelId,
                    text = $"<pre>{escaped}</pre>",
                    parse_mode = "HTML",
                    link_preview_options = new { is_disabled = true }
                };

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    SiteTools.AdminTelegramWorkerSendUrl);

                request.Headers.Add(
                    "Authorization",
                    $"Bearer {SiteTools.AdminTelegramWorkerKey}");

                request.Content = JsonContent.Create(payload);

                await _httpClient.SendAsync(request);
            }
            catch
            {
                // Swallow on purpose — alerting must never throw.
            }
        }
    }
}
