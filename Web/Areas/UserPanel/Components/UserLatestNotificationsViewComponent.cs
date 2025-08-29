using Application.Services.Interfaces;
using Application.ViewModels.SiteNotifications.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserLatestNotificationsViewComponent : ViewComponent
    {
        private readonly ISiteNotificationService _notificationService;
        public UserLatestNotificationsViewComponent(ISiteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _notificationService.ClientSideFilterNotifications(new ClientSideFilterNotificationsViewModel());
            if (result.IsFailure) return View("UserLatestNotifications", new ClientSideFilterNotificationsViewModel());

            return View("UserLatestNotifications", result.Value);
        }
    }
}
