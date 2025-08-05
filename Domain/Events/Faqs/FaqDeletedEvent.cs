using MediatR;

namespace Domain.Events.Faqs
{
    public record FaqDeletedEvent(
        int id
        ) : INotification;
}
