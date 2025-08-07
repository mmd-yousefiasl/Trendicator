using Domain.Enums.SiteInfo;
using MediatR;

namespace Domain.Events.SiteInfo.SocialMedia
{
    public record SocialMediaUpdatedEvent(
    int id,
    string Url,
    int Priority,
    bool IsPublished,
    SocialMediaTypeEnum Type
    ) : INotification;
}
