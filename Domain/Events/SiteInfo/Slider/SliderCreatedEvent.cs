using MediatR;

namespace Domain.Events.SiteInfo.Slider
{
    public record SliderCreatedEvent(
    string Banner,
    string SubTitle,
    string Title,
    string ButtonTitle,
    string Url,
    int Priority,
    bool IsPublished
    ) : INotification;
}
