using Application.Services.Interfaces;
using Application.Static;
using Domain.Events.Faqs;
using MediatR;

namespace Application.Handlers.Faq
{
    public class FaqCacheHandlers : INotificationHandler<FaqCreatedEvent>,
        INotificationHandler<FaqUpdatedEvent>,
        INotificationHandler<FaqDeletedEvent>
    {
        private readonly ICacheManager _cacheManager;
        public FaqCacheHandlers(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Handle(FaqCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.FaqCacheKey);
        }

        public Task Handle(FaqUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.FaqCacheKey);
        }

        public Task Handle(FaqDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.FaqCacheKey);
        }
    }
}
