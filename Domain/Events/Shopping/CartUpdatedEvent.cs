using Domain.Enums.Shopping;
using MediatR;

namespace Domain.Events.Shopping
{
    public record CartUpdatedEvent(
    int id,
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
