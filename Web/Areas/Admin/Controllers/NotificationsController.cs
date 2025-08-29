using Application.Services.Interfaces;
using Application.ViewModels.SiteNotifications.AdminSide;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageNotifications")]
    public class NotificationsController : BaseAdminController
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

        [Permission("NotificationsList")]
        [HttpGet("/admin/notifications-list")]
        public async Task<IActionResult> NotificationsList(AdminSideFilterNotificationsViewModel filter)
        {
            var result = await _notificationService.AdminSideFilterNotifications(filter);
            if (result.IsFailure) return View(new AdminSideFilterNotificationsViewModel());

            return View(result.Value);
        }
    }
}
