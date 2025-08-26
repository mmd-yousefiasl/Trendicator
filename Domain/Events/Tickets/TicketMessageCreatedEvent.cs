using MediatR;

namespace Domain.Events.Tickets
{
    public record TicketMessageCreatedEvent(
    string Message,
    bool IsSentByUser,
    int TicketId
    ) : INotification;
}
