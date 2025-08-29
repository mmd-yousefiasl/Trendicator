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
    long? TotalPaid,
    long? TotalDiscount,
    string? DiscountCode,
    long? SubTotal,
    long? Tax,
    DateTime? PaidDate
    ) : INotification;
}
