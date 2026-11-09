using Application.Dtos.SiteNotifications.AdminSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Enums.SiteNotifications;
using Domain.Events.Tickets;
using MediatR;

namespace Application.Handlers
{
    public class TicketNotificationHandlers : INotificationHandler<TicketCreatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public TicketNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            var ticketNotif = new AdminSideCreateNewNotificationDto()
            {
                Type = SiteNotificationTypeEnum.NewTicket,
                Content = $"تیکت جدیدی با عنوان «{notification.Title}» " +
                          $"در دسته‌بندی «{notification.Category.GetDisplayName()}» " +
                          $"با اولویت «{notification.Priority.GetDisplayName()}» " +
                          $"و وضعیت «{notification.Status.GetDisplayName()}» " +
                          $"در تاریخ {notification.CreatedDate.GetDateTime()} ثبت شد."
            };

            await _notificationService.AdminSideCreateNewNotification(ticketNotif);
        }
    }
}
