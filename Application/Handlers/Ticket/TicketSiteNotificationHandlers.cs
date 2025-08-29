using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SiteNotifications.AdminSide;
using Application.ViewModels.SiteNotifications.ClientSide;
using Domain.Events.Tickets;
using MediatR;

namespace Application.Handlers.Ticket
{
    public class TicketSiteNotificationHandlers : INotificationHandler<TicketCreatedEvent>,
         INotificationHandler<TicketUpdatedEvent>
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public TicketSiteNotificationHandlers(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            var content = "تیکت پشتیبانی جدید ثبت شد";

            await _notificationService.AdminSideCreateNewNotification(new AdminSideCreateNewNotificationViewModel()
            {
                Content = content,
            });

            await _notificationService.ClientSideCreateNewNotification(new ClientSideCreateNewNotificationViewModel()
            {
                UserId = notification.UserId,
                Content = content
            });
        }

        public async Task Handle(TicketUpdatedEvent notification, CancellationToken cancellationToken)
        {

            var content = $"تیکت با شناسه {notification.id} به {notification.Status.GetDisplayName()} به روزرسانی شد";

            await _notificationService.ClientSideCreateNewNotification(new ClientSideCreateNewNotificationViewModel()
            {
                UserId = notification.UserId,
                Content = content
            });
        }
    }
}
