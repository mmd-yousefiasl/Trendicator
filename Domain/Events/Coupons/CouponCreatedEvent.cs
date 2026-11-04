using MediatR;

namespace Domain.Events.Coupons
{
    public record CouponCreatedEvent(
    string Title,
    DateTime FromDate,
    DateTime ToDate,
    string Code,
    int Percent
    ) : INotification;
}
