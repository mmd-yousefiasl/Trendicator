using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SiteNotifications.AdminSide;
using Application.ViewModels.SiteNotifications.ClientSide;
using Domain.Events.Shopping;
using MediatR;

namespace Application.Handlers.Cart
{
    public class CartSiteNotificationHandlers : INotificationHandler<CartUpdatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public CartSiteNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(CartUpdatedEvent notification, CancellationToken cancellationToken)
        {
            string content = $"سفارش با شناسه {notification.id} ثبت شد! وضعیت جدید {notification.Status.GetDisplayName()}";

            var adminNotification = new AdminSideCreateNewNotificationViewModel
            {
                Content = content
            };

            await _notificationService.AdminSideCreateNewNotification(adminNotification);

            if (notification.UserId > 0)
            {
                var userNotification = new ClientSideCreateNewNotificationViewModel()
                {
                    UserId = notification.UserId.Value,
                    Content = content
                };

                await _notificationService.ClientSideCreateNewNotification(userNotification);
            }
        }
    }
}
