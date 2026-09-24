using Application.Dtos;

namespace Application.Dtos.SiteNotifications.ClientSide
{
    public class ClientSideNotificationDto : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
    }
}
