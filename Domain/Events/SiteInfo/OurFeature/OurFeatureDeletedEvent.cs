using MediatR;

namespace Domain.Events.SiteInfo.OurFeature
{
    public record OurFeatureDeletedEvent(
        int id
        ) : INotification;
}
