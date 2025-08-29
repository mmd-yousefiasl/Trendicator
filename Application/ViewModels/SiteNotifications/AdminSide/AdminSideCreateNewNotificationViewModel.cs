using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteNotifications.AdminSide
{
    public class AdminSideCreateNewNotificationViewModel
    {
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
