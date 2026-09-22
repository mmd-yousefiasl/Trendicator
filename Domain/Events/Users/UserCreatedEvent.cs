using MediatR;

namespace Domain.Events.Users
{
    public record UserCreatedEvent(
    string Mobile
    ) : INotification;
}
