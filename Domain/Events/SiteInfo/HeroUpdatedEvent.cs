using MediatR;

namespace Domain.Events.SiteInfo
{
    public record HeroUpdatedEvent(
    int Id,
    string Title,
    string Description,
    string FileName
    ) : INotification;
}
