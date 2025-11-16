using Application.Services.Interfaces;
using Application.Static;
using Domain.Events.SiteInfo;
using Domain.Events.SiteInfo.Banner;
using Domain.Events.SiteInfo.OurFeature;
using Domain.Events.SiteInfo.SocialMedia;
using Domain.Events.SiteInfo.Testimonial;
using MediatR;

namespace Application.Handlers.SiteInfo
{
    public class SiteInfoCacheHandlers : INotificationHandler<BannerCreatedEvent>,
        INotificationHandler<BannerUpdatedEvent>,
        INotificationHandler<BannerDeletedEvent>,
        INotificationHandler<EmailDataUpdatedEvent>,
        INotificationHandler<OurFeatureCreatedEvent>,
        INotificationHandler<OurFeatureUpdatedEvent>,
        INotificationHandler<OurFeatureDeletedEvent>,
        INotificationHandler<PaymentDataUpdatedEvent>,
        INotificationHandler<SiteDataUpdatedEvent>,
        INotificationHandler<SocialMediaCreatedEvent>,
        INotificationHandler<SocialMediaUpdatedEvent>,
        INotificationHandler<SocialMediaDeletedEvent>,
        INotificationHandler<TestimonialCreatedEvent>,
        INotificationHandler<TestimonialUpdatedEvent>,
        INotificationHandler<TestimonialDeletedEvent>,
        INotificationHandler<SmsDataUpdatedEvent>,
        INotificationHandler<HeroUpdatedEvent>
    {
        private readonly ICacheManager _cacheManager;
        public SiteInfoCacheHandlers(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Handle(BannerCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.BannerCacheKey);
        }

        public Task Handle(BannerUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.BannerCacheKey);
        }

        public Task Handle(BannerDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.BannerCacheKey);
        }

        public Task Handle(EmailDataUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.EmailDataCacheKey);
        }

        public Task Handle(OurFeatureCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.OurFeatureCacheKey);
        }

        public Task Handle(OurFeatureUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.OurFeatureCacheKey);
        }

        public Task Handle(OurFeatureDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.OurFeatureCacheKey);
        }

        public Task Handle(PaymentDataUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.PaymentDataCacheKey);
        }

        public Task Handle(SiteDataUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.SiteDataCacheKey);
        }

        public Task Handle(SocialMediaCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.SocialMediaCacheKey);
        }

        public Task Handle(SocialMediaUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.SocialMediaCacheKey);
        }

        public Task Handle(SocialMediaDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.SocialMediaCacheKey);
        }

        public Task Handle(TestimonialCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.TestimonialCacheKey);
        }

        public Task Handle(TestimonialUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.TestimonialCacheKey);
        }

        public Task Handle(TestimonialDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.TestimonialCacheKey);
        }

        public Task Handle(SmsDataUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.SmsDataCacheKey);
        }

        public Task Handle(HeroUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.HeroCacheKey);
        }
    }
}
