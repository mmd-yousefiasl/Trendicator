using MediatR;

namespace Domain.Events.Users
{
    public record UserLoggedInEvent(
    int Id
    ) : INotification;
}