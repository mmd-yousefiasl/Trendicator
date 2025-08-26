using Domain.Enums.Tickets;
using MediatR;

namespace Domain.Events.Tickets
{
    public record TicketUpdatedEvent(
    int id,
    string Subject,
    TicketStatusEnum Status,
    int UserId
    ) : INotification;
}
