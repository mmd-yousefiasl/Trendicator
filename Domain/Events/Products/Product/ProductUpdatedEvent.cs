using Domain.Enums.Products;
using MediatR;

namespace Domain.Events.Products.Product
{
    public record ProductUpdatedEvent(
    int Id,
    string BannerFileName,
    string Title,
    string Slug,
    string ShortDescription,
    string Description,
    int Priority,
    bool IsPublished,
    long DisplayPrice,
    long? DisplayOrderPrice,
    bool IsAvailable,
    ProductTypeEnum Type
    ) : INotification;
}
