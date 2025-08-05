using MediatR;

namespace Domain.Events.SiteInfo.OurFeature
{
    public record OurFeatureUpdatedEvent(
    int id,
    string Title,
    string Description,
    string FileName,
    int Priority,
    bool IsPublished
    ) : INotification;
}
