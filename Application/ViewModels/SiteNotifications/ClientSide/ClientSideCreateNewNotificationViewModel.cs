using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteNotifications.ClientSide
{
    public class ClientSideCreateNewNotificationViewModel
    {
        [MaxLength(1000)]
        public string Content { get; set; }

        public int UserId { get; set; }
    }
}
