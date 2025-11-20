using Application.Services.Interfaces;
using Application.Static;
using Domain.Events.Products.ProductCategory;
using MediatR;

namespace Application.Handlers.Product.ProductCategory
{
    public class ProductCategoryCacheHandlers : INotificationHandler<ProductCategoryCreatedEvent>,
        INotificationHandler<ProductCategoryUpdatedEvent>,
        INotificationHandler<ProductCategoryDeletedEvent>
    {
        private readonly ICacheManager _cacheManager;
        public ProductCategoryCacheHandlers(ICacheManager cacheManger)
        {
            _cacheManager = cacheManger;
        }

        public Task Handle(ProductCategoryCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.ProductCategoriesCacheKey);
        }

        public Task Handle(ProductCategoryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.ProductCategoriesCacheKey);
        }

        public Task Handle(ProductCategoryDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _cacheManager.RemoveByKeyAsync(CacheKeys.ProductCategoriesCacheKey);
        }
    }
}
