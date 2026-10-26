using Domain.Enums.Products;
using MediatR;

namespace Domain.Events.Products.Product
{
    public record ProductCreatedEvent(
    string BannerFileName,
    string Title,
    string Slug,
    string ShortDescription,
    string Description,
    int Priority,
    bool IsPublished,
    decimal DisplayPrice,
    decimal OrderPrice,
    bool IsAvailable,
    ProductTypeEnum Type,
    string? MetaTraderLink,
    string? IndicatorFileName
    ) : INotification;
}
