using Domain.Enums.ContactUs;
using MediatR;

namespace Domain.Events.ContactUs
{
    public record ContactUsFormCreatedEvent(
    DateTime CreatedDate,
    string FullName,
    string Mobile,
    ContactUsFormTypeEnum Type,
    string Message,
    int? UserId,
    string UserIP
    ) : INotification
    {
        public object Create { get; set; }
    }
}
