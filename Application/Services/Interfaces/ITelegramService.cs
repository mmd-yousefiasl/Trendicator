using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ITelegramService
    {
        Task<Result> SendSiteNotificationToChannelAsync(string content);
    }
}
