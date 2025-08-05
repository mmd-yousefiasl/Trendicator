using MediatR;

namespace Domain.Events.SiteInfo.Banner
{
    public record BannerDeletedEvent(
        int Id
        ) : INotification;
}
