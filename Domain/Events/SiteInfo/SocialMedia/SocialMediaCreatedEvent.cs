using Domain.Enums.SiteInfo;
using MediatR;

namespace Domain.Events.SiteInfo.SocialMedia
{
    public record SocialMediaCreatedEvent(
    string Url,
    int Priority,
    bool IsPublished,
    SocialMediaTypeEnum Type
    ) : INotification;
}
