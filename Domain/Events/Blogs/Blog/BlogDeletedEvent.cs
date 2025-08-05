using MediatR;

namespace Domain.Events.Blogs.Blog
{
    public record BlogDeletedEvent(
        int id
        ) : INotification;
}
