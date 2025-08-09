using MediatR;

namespace Domain.Events.NewsLetters
{
    public record NewsLetterCreatedEvent(
    string Email,
    string UserIP,
    int? UserId
    ) : INotification;
}
