using MediatR;

namespace Domain.Events.SiteInfo.Slider
{
    public record SliderDeletedEvent(
        int Id
        ) : INotification;
}
