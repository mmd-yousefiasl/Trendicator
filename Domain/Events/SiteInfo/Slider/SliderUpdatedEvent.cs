using MediatR;

namespace Domain.Events.SiteInfo.Slider
{
    public record SliderUpdatedEvent(
    int Id,
    string Banner,
    string SubTitle,
    string Title,
    string ButtonTitle,
    string Url,
    int Priority,
    bool IsPublished
    ) : INotification;
}
