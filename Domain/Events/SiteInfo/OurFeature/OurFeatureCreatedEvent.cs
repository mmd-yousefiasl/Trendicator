using MediatR;

namespace Domain.Events.SiteInfo.OurFeature
{
    public record OurFeatureCreatedEvent(
    string Title,
    string Description,
    string FileName,
    int Priority,
    bool IsPublished
    ) : INotification;
}
