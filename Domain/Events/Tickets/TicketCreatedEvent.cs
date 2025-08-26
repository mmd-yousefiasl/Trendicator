using Domain.Enums.Tickets;
using MediatR;

namespace Domain.Events.Tickets
{
    public record TicketCreatedEvent(
    string Subject,
    TicketStatusEnum Status,
    int UserId
    ) : INotification;
}
