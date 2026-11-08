using Application.Dtos.SiteNotifications.AdminSide;
using Application.Dtos.SiteNotifications.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Events.Shopping;
using MediatR;

namespace Application.Handlers.Cart
{
    public class CartPaidHandlers : INotificationHandler<CartPaidEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        private readonly ICartService _cartService;
        #endregion

        #region Ctor
        public CartPaidHandlers(ISiteNotificationService notificationService,
            ICartService cartService)
        {
            _notificationService = notificationService;
            _cartService = cartService;
        }
        #endregion

        public async Task Handle(CartPaidEvent notification, CancellationToken cancellationToken)
        {
            string content = $"سفارش با شناسه {notification.Id} ثبت شد! وضعیت جدید {notification.Status.GetDisplayName()}";

            var adminNotification = new AdminSideCreateNewNotificationDto
            {
                Content = content
            };

            await _notificationService.AdminSideCreateNewNotification(adminNotification);

            if (notification.UserId > 0)
            {
                var userNotification = new ClientSideCreateNewNotificationDto()
                {
                    UserId = notification.UserId.Value,
                    Content = content
                };

                await _notificationService.ClientSideCreateNewNotification(userNotification);
                await _cartService.UserOrderStatsNeedUpdate(notification.UserId.Value);
            }
        }
    }
}
