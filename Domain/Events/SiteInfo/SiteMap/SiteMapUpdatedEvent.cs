using Domain.Enums.SiteInfo;
using MediatR;

namespace Domain.Events.SiteInfo.SiteMap
{
    public record SiteMapUpdatedEvent(
    int Id,
    string Url,
    ChangeFreqEnum ChagneFreq,
    decimal Priority,
    bool IsPublished,
    SiteMapTypeEnum Type
    ) : INotification;
}
