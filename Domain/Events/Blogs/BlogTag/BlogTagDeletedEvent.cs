using MediatR;

namespace Domain.Events.Blogs.BlogTag
{
    public record BlogTagDeletedEvent(
        int id
        ) : INotification;
}
