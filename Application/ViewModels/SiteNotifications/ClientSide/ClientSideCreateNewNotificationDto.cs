using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteNotifications.ClientSide
{
    public class ClientSideCreateNewNotificationDto
    {
        [MaxLength(1000)]
        public string Content { get; set; }

        public int UserId { get; set; }
    }
}
