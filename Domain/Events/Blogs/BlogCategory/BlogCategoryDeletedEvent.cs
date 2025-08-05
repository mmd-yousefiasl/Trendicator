using MediatR;

namespace Domain.Events.Blogs.BlogCategory
{
    public record BlogCategoryDeletedEvent(
        int id
        ) : INotification;
}
