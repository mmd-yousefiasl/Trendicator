using MediatR;

namespace Domain.Events.SiteInfo.Testimonial
{
    public record TestimonialCreatedEvent(
    string Content,
    string Writer,
    string Position,
    int Priority,
    bool IsPublished
    ) : INotification;
}
