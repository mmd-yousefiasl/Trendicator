using MediatR;

namespace Domain.Events.Blogs.BlogTag
{
    public record BlogTagUpdatedEvent(
        int id,
        string Title
        ) : INotification;
}
