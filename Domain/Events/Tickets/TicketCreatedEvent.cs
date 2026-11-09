using Domain.Enums.Tickets;
using MediatR;

namespace Domain.Events.Tickets
{
    public record TicketCreatedEvent(
    DateTime CreatedDate,
    string Title,
    TicketCategoryEnum Category,
    TicketPriorityEnum Priority,
    TicketStatusEnum Status
    ) : INotification;
}
