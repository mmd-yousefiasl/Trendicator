using MediatR;

namespace Domain.Events.Products.ProductCategory
{
    public record ProductCategoryUpdatedEvent(
        int Id,
        string Title
        ) : INotification;
}
