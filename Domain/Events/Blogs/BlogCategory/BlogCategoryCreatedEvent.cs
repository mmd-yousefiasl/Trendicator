using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryCreatedEvent(
    string Title,
    string Slug,
    int Priority,
    bool IsPublished
    ) : INotification;
}
