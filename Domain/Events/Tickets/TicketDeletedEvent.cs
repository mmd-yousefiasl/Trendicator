using MediatR;

namespace Domain.Events.Tickets
{
    public record TicketDeletedEvent(
        int id
        ) : INotification;
}
