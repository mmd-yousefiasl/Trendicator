using Domain.Enums.SiteNotifications;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteNotifications.AdminSide
{
    public class AdminSideFilterNotificationsDto : BasePaging<AdminSideNotificationDto>
    {
        [Display(Name = "نوع اعلان")]
        public SiteNotificationTypeEnum? Type { get; set; }
    }
}
