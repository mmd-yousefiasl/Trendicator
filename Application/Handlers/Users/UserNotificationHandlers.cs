using Application.Services.Interfaces;
using Application.ViewModels.SiteNotifications.AdminSide;
using Domain.Enums.SiteNotifications;
using Domain.Events.Users;
using MediatR;

namespace Application.Handlers.Users
{
    public class UserNotificationHandlers : INotificationHandler<UserCreatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public UserNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var newUserNotif = new AdminSideCreateNewNotificationDto()
            {
                Type = SiteNotificationTypeEnum.NewUser,
                Content = $"کاربر جدیدی با شماره موبایل {notification.Mobile} ثبت‌نام کرد."
            };

            await _notificationService.AdminSideCreateNewNotification(newUserNotif);
        }
    }
}