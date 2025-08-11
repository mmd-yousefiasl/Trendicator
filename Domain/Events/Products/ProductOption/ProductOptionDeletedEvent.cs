using MediatR;

namespace Domain.Events.Products.ProductOption
{
    public record ProductOptionDeletedEvent(
        int Id
        ) : INotification;
}
