using MediatR;

namespace Domain.Events.SiteInfo.SiteMap
{
    public record SiteMapDeletedEvent(
    int Id
    ) : INotification;
}
