using Application.Services.Interfaces;
using Application.Static;
using Domain.Events.Coupons;
using MediatR;

namespace Application.Handlers.Coupons
{
    public class CouponCacheHandlers : INotificationHandler<CouponCreatedEvent>,
        INotificationHandler<CouponUpdatedEvent>,
        INotificationHandler<CouponDeletedEvent>
    {
        #region Fields
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public CouponCacheHandlers(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        #endregion

        public Task Handle(CouponCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.CouponCacheKey);
        }

        public Task Handle(CouponUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.CouponCacheKey);
        }

        public Task Handle(CouponDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.CouponCacheKey);
        }
    }
}
