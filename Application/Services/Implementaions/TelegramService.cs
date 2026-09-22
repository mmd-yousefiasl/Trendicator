using Application.Services.Interfaces;
using Domain.Shared;
using Domain.Statics;
using System.Net.Http.Json;

namespace Application.Services.Implementaions
{
    public class TelegramService : ITelegramService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Ctor
        public TelegramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        public Task<Result> SendSiteNotificationToChannelAsync(string content)
        {
            if (SiteTools.TelegramNotificationChannelId == 0 ||
                string.IsNullOrWhiteSpace(content))
            {
                return Task.FromResult(Result.Failure(Messages.OperationFailedError));
            }

            string message = $"""
    📢 <b>اعلان جدید از دیجیتو</b>

    {content}
    """;

            return PostMessageAsync(SiteTools.AdminTelegramWorkerSendUrl,
                SiteTools.AdminTelegramWorkerKey,
                SiteTools.TelegramNotificationChannelId,
                message);
        }

        private async Task<Result> PostMessageAsync(string TelegramWorkerSendUrl, string TelegramWorkerKey, long chatId, string messageText)
        {
            var payload = new
            {
                chat_id = chatId,
                text = messageText,
                parse_mode = "HTML",

                // Telegram expands the product URL into a huge photo preview —
                // keep alert messages compact.
                link_preview_options = new { is_disabled = true }
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                TelegramWorkerSendUrl);

            request.Headers.Add(
                "Authorization",
                $"Bearer {TelegramWorkerKey}");

            request.Content = JsonContent.Create(payload);

            try
            {
                await _httpClient.SendAsync(request);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Messages.OperationFailedError);
            }
        }
    }
}
