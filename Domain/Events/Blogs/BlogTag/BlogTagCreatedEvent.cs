using MediatR;

namespace Domain.Events.Blogs.BlogTag
{
    public record BlogTagCreatedEvent(
        string Title
        ) : INotification;
}
