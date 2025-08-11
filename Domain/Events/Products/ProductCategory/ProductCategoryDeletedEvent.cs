using MediatR;

namespace Domain.Events.Products.ProductCategory
{
    public record ProductCategoryDeletedEvent(
        int Id
        ) : INotification;
}
