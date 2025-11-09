using MediatR;

namespace Domain.Events.SiteInfo
{
    public record SmsDataUpdatedEvent(
    string? SmsPanelAPIKey,
    int? SmsPatternIdForLoginCode
    ) : INotification;
}
