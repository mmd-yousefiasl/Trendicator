using Domain.Enums.Shopping;
using MediatR;

namespace Domain.Events.Shopping
{
    public record CartUpdatedEvent(
    int Id,
    CartStatusEnum Status,
    decimal? TotalPaid,
    decimal? TotalDiscount,
    string? DiscountCode,
    DateTime? PaidDate,
    int? UserId,
    string? VisitorId
    ) : INotification;
}
