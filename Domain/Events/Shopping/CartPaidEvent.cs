using Domain.Enums.Shopping;
using MediatR;

namespace Domain.Events.Shopping
{
    public record CartPaidEvent(
    int Id,
    CartStatusEnum Status,
    decimal? TotalPaid,
    decimal? TotalDiscount,
    string? DiscountCode,
    DateTime? PaidDate,
    int? UserId,
    string? VisitorId,
    List<int>? ProductIds
    ) : INotification;
}
