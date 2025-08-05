using MediatR;

namespace Domain.Events.SiteInfo.Banner
{
    public record BannerCreatedEvent(
    string FileName,
    string Title,
    string Url,
    int Priority,
    bool IsPublished
    ) : INotification;
}
