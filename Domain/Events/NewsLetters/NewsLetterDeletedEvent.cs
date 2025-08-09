using MediatR;

namespace Domain.Events.NewsLetters
{
    public record NewsLetterDeletedEvent(
    int id
    ) : INotification;
}
