using MediatR;

namespace Domain.Events.Products.ProductOption
{
    public record ProductOptionCreatedEvent(
    string Title,
    long Price,
    long? OrderPrice
    ) : INotification;
}
