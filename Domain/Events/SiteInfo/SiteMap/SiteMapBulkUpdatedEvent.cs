using MediatR;

namespace Domain.Events.SiteInfo.SiteMap
{
    public record SiteMapBulkUpdatedEvent(
    bool Trigger
    ) : INotification;
}
