using MediatR;

namespace Domain.Events.NewsLetters
{
    public record SentLetterCreatedEvent(
    string Subject,
    string Message,
    int TargetMailCount,
    bool IsSucceed
    ) : INotification;
}
