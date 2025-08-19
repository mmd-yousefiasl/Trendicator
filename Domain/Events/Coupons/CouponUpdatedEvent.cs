using MediatR;

namespace Domain.Events.Coupons
{
    public record CouponUpdatedEvent(
    int Id,
    string Title,
    DateTime FromDate,
    DateTime ToDate,
    string Code,
    int Percent,
    List<int>? UserIds
    ) : INotification;
}
