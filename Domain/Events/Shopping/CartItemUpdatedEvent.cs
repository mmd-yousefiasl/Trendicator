using Domain.Enums.Products;
using Domain.Enums.Shopping;
using MediatR;

namespace Domain.Events.Shopping
{
    public record CartItemUpdatedEvent(
    string Title,
    decimal OrderPrice,
    decimal DisplayPrice ,
    ProductTypeEnum ProductType,
    string? TradingViewAccountName ,
    CartItemStatus Status ,
    int CartId ,
    int ProductId  
    ) : INotification;
}
