using Domain.Enums.Shopping;
using MediatR;

namespace Domain.Events.Shopping
{
    public record CartCreatedEvent(
    string UserIP,
    int? UserId,
    CartStatusEnum Status,
    bool IsPaid,
    decimal? TotalPaid,
    decimal? TotalDiscount,
    string? DiscountCode,
    decimal? SubTotal,
    decimal? Tax,
    DateTime? PaidDate
    ) : INotification;
}
