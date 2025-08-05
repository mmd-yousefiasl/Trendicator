using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryUpdatedEvent(
    int id,
    string Title,
    int Priority,
    bool IsPublished
    ) : INotification;
}
