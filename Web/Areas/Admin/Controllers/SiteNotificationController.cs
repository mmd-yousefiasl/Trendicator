using Application.Dtos.SiteNotifications.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageSiteNotifications")]
    public class SiteNotificationController : BaseAdminController
    {
        #region Fields
        private readonly ISiteNotificationService _notificationService;
        #endregion

        #region Ctor
        public SiteNotificationController(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        [Permission("NotificationsList")]
        [HttpGet("/admin/notifications-list")]
        public async Task<IActionResult> NotificationsList(AdminSideFilterNotificationsDto filter)
        {
            var result = await _notificationService.AdminSideFilterNotifications(filter);
            if (result.IsFailure)
                return View(new AdminSideFilterNotificationsDto());

            await _notificationService.AdminSideSetNotificationsRead();

            return View(result.Value);
        }
    }
}
