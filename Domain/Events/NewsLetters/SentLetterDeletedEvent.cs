using MediatR;

namespace Domain.Events.NewsLetters
{
    public record SentLetterDeletedEvent(
    int id
    ) : INotification;
}
