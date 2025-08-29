using Application.Services.Interfaces;
using Application.ViewModels.SiteNotifications.AdminSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class LatestNotificationsViewComponent : ViewComponent
    {
        private readonly ISiteNotificationService _notificationService;
        public LatestNotificationsViewComponent(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _notificationService.AdminSideGetLatestNotifications();
            if (result.IsFailure) return View("LatestNotifications", new List<AdminSideNotificationViewModel>());

            return View("LatestNotifications", result.Value);
        }
    }
}
