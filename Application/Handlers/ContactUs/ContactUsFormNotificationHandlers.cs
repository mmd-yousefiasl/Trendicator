using Application.Dtos.SiteNotifications.AdminSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Enums.SiteNotifications;
using Domain.Events.ContactUs;
using MediatR;

namespace Application.Handlers.ContactUs
{
    public class ContactUsFormNotificationHandlers : INotificationHandler<ContactUsFormCreatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public ContactUsFormNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(ContactUsFormCreatedEvent notification, CancellationToken cancellationToken)
        {
            var contactUsFormNotif = new AdminSideCreateNewNotificationDto()
            {
                Type = SiteNotificationTypeEnum.NewContactUsForm,
                Content = $"پیام تماس با ما از طرف «{notification.FullName}» " +
                          $"با شماره موبایل {notification.Mobile} " +
                          $"با موضوع «{notification.Type.GetDisplayName()}» " +
                          $"در تاریخ {notification.CreatedDate.GetDateTime()} ثبت شد. " +
                          $"پیام: {(notification.Message.Length > 100 ? notification.Message[..100] + "..." : notification.Message)}"
            };

            await _notificationService.AdminSideCreateNewNotification(contactUsFormNotif);
        }
    }
}
