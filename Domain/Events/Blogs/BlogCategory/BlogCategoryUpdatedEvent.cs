using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryUpdatedEvent(
    int id,
    string Title,
    string Slug,
    string MetaDescription,
    int Priority,
    bool IsPublished
    ) : INotification;
}
