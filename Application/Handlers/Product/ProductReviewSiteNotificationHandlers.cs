using Application.Dtos.SiteNotifications.AdminSide;
using Application.Services.Interfaces;
using Domain.Events.Products.ProductReview;
using MediatR;

namespace Application.Handlers.Product
{
    public class ProductReviewSiteNotificationHandlers : INotificationHandler<ProductReviewCreatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public ProductReviewSiteNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(ProductReviewCreatedEvent notification, CancellationToken cancellationToken)
        {
            var content = "یک بررسی جدید برای محصول اضافه شد";

            await _notificationService.AdminSideCreateNewNotification(new AdminSideCreateNewNotificationDto()
            {
                Content = content
            });
        }
    }
}
