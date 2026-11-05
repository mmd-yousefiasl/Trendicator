using MediatR;

namespace Domain.Events.SiteInfo
{
    public record PaymentDataUpdatedEvent(
        int Id,
        string PaymentGatewayPin
        ) : INotification;
}
