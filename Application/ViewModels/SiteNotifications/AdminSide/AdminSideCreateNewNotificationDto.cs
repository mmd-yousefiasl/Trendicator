using Domain.Enums.SiteNotifications;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SiteNotifications.AdminSide
{
    public class AdminSideCreateNewNotificationDto
    {
        [Display(Name = "اعلان")]
        [MaxLength(2500)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Content { get; set; }

        [Display(Name = "نوع اعلان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public SiteNotificationTypeEnum Type { get; set; }
    }
}
