using Domain.Enums.SiteInfo;
using MediatR;

namespace Domain.Events.SiteInfo.SiteMap
{
    public record SiteMapCreatedEvent(
    string Url,
    ChangeFreqEnum ChagneFreq,
    decimal Priority,
    bool IsPublished,
    SiteMapTypeEnum Type
    ) : INotification;
}
