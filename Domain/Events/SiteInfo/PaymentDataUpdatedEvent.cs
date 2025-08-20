using MediatR;

namespace Domain.Events.SiteInfo
{
    public record PaymentDataUpdatedEvent(
        int Id,
        int Tax
        ) : INotification;
}
