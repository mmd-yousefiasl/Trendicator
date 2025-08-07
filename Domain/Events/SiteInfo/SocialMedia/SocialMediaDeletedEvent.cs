using MediatR;

namespace Domain.Events.SiteInfo.SocialMedia
{
    public record SocialMediaDeletedEvent(
        int id
        ) : INotification;
}
