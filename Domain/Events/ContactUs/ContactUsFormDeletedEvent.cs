using MediatR;

namespace Domain.Events.ContactUs
{
    public record ContactUsFormDeletedEvent(
        int id
        ) : INotification;
}
