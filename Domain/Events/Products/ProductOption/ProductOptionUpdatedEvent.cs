using MediatR;

namespace Domain.Events.Products.ProductOption
{
    public record ProductOptionUpdatedEvent(
    int Id,
    string Title,
    long Price,
    long? OrderPrice
    ) : INotification;
}
