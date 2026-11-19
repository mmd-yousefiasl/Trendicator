using Application.Dtos.SiteNotifications.AdminSide;
using Application.Services.Interfaces;
using Domain.Enums.Shopping;
using Domain.Enums.SiteNotifications;
using Domain.Events.Shopping;
using Domain.Statics;
using MediatR;

namespace Application.Handlers.Cart
{
    public class CartItemNotificationHandlers : INotificationHandler<CartItemUpdatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public CartItemNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(CartItemUpdatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Status != CartItemStatus.InProgress)
                return;

            var orderLink = $"{SiteTools.SiteUrl}admin/order-details/{notification.CartId}";

            var content =
                $"کاربر اکانت TradingView خود را ثبت کرد و نیاز به فعال‌سازی دارد. " +
                $"<a href=\"{orderLink}\" target=\"_blank\">مشاهده سفارش</a>";

            await _notificationService.AdminSideCreateNewNotification(new AdminSideCreateNewNotificationDto
            {
                Type = SiteNotificationTypeEnum.CartItemNeedsReview,
                Content = content,
            });
        }
    }
}