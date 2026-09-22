using Domain.Enums.SiteNotifications;

namespace Domain.Models.SiteNotifications
{
    public class SiteNotification : AuditBaseEntity<int>
    {
        public string Content { get; set; }
        public int? ForUserId { get; set; }
        public bool IsRead { get; set; }
        public SiteNotificationTypeEnum Type { get; set; }
    }
}
