using Domain.Enums.SiteNotifications;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SiteNotifications.AdminSide
{
    public class AdminSideFilterNotificationsDto : BasePaging<AdminSideNotificationDto>
    {
        [Display(Name = "نوع اعلان")]
        public SiteNotificationTypeEnum? Type { get; set; }
    }
}
