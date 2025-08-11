using MediatR;

namespace Domain.Events.Products.Product
{
    public record ProductDeletedEvent(
        int Id
        ) : INotification;
}
