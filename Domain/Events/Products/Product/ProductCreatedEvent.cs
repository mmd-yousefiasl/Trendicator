using MediatR;

namespace Domain.Events.Products.Product
{
    public record ProductCreatedEvent(
    string BannerFileName,
    string Title,
    string Slug,
    string Description,
    int Priority,
    bool IsPublished,
    long DisplayPrice,
    long? DisplayOrderPrice,
    bool IsAvailable
    ) : INotification;
}
