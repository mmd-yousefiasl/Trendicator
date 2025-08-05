using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryCreatedEvent(
    string Title,
    int Priority,
    bool IsPublished
    ) : INotification;
}
