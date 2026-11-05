using Application.Dtos.SiteNotifications.AdminSide;
using Application.Dtos.SiteNotifications.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Events.Shopping;
using MediatR;

namespace Application.Handlers.Cart
{
    public class CartNotificationHandlers : INotificationHandler<CartPaidEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public CartNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
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
            }
        }
    }
}
