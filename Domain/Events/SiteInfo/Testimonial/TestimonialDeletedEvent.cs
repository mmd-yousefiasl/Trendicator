using MediatR;

namespace Domain.Events.SiteInfo.Testimonial
{
    public record TestimonialDeletedEvent(
        int Id
        ) : INotification;
}
