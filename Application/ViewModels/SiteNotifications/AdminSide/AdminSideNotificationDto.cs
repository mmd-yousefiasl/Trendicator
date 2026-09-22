namespace Application.ViewModels.SiteNotifications.AdminSide
{
    public class AdminSideNotificationDto : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
    }
}
