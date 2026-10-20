using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryCreatedEvent(
    string Title,
    string Slug,
    string MetaDescription,
    int Priority,
    bool IsPublished
    ) : INotification;
}
