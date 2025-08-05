using MediatR;

namespace Domain.Events.Faqs
{
    public record FaqCreatedEvent(
    string Question,
    string Answer,
    int Priority,
    bool IsPublished
    ) : INotification;
}
