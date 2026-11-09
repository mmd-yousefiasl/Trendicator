using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Tickets
{
    public enum TicketStatusEnum
    {
        [Display(Name = "باز شده")]
        Open = 1,

        [Display(Name = "در دست بررسی")]
        Pending = 2,

        [Display(Name = "پاسخ داده شده")]
        Answered = 3,

        [Display(Name = "پاسخ کاربر")]
        UserReplied = 4,

        [Display(Name = "بسته شده")]
        Closed = 5
    }
}
