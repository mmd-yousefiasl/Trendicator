using MediatR;

namespace Domain.Events.SiteInfo
{
    public record EmailDataUpdatedEvent(
    int id,
    string? From,
    string? Password,
    string? EmailDisplayName,
    string? Smtp,
    int? Port
    ) : INotification;
}
