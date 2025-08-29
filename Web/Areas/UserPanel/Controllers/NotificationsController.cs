using Application.Services.Interfaces;
using Application.ViewModels.SiteNotifications.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class NotificationsController : BaseUserPanelController
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public NotificationsController(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        [HttpGet("/user/notifications-list")]
        public async Task<IActionResult> NotificationsList(ClientSideFilterNotificationsViewModel filter)
        {
            var result = await _notificationService.ClientSideFilterNotifications(filter);
            if (result.IsFailure) return View(new ClientSideFilterNotificationsViewModel());

            return View(result.Value);
        }
    }
}
