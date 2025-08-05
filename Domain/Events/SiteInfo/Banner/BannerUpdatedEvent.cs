using MediatR;

namespace Domain.Events.SiteInfo.Banner
{
    public record BannerUpdatedEvent(
    int Id,
    string FileName,
    string Title,
    string Url,
    int Priority,
    bool IsPublished
    ) : INotification;
}