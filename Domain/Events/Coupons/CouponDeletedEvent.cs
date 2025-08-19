using MediatR;

namespace Domain.Events.Coupons
{
    public record CouponDeletedEvent(
        int Id
        ) : INotification;
}
