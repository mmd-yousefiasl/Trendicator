using MediatR;

namespace Domain.Events.Products.ProductCategory
{
    public record ProductCategoryCreatedEvent(
        string Title
        ) : INotification;
}
