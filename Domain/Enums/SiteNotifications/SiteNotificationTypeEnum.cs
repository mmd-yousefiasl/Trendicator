using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.SiteNotifications
{
    public enum SiteNotificationTypeEnum
    {
        [Display(Name = "ثبت نام کاربر جدید")]
        NewUser = 1,
        [Display(Name = "سفارش جدید")]
        NewOrder = 2,
        [Display(Name = "فرم تماس باما جدید")]
        NewContactUsForm = 3,
        [Display(Name = "تیکت جدید")]
        NewTicket = 4,
    }
}
