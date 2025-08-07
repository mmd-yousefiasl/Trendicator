using MediatR;

namespace Domain.Events.SiteInfo.Testimonial
{
    public record TestimonialUpdatedEvent(
    int Id,
    string Content,
    string Writer,
    string Position,
    int Priority,
    bool IsPublished
    ) : INotification;
}
