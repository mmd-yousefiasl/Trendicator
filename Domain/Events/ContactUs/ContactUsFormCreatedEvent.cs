using MediatR;

namespace Domain.Events.ContactUs
{
    public record ContactUsFormCreatedEvent(
    string FullName,
    string Email,
    string PhoneNumber,
    string Subject,
    string Message,
    int? UserId,
    string UserIP
    ) : INotification;
}
