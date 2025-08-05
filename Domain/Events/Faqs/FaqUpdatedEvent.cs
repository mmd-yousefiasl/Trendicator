using MediatR;

namespace Domain.Events.Faqs
{
    public record FaqUpdatedEvent(
    int id,
    string Question,
    string Answer,
    int Priority,
    bool IsPublished
    ) : INotification;
}
