using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Tickets
{
    public enum TicketStatusEnum
    {
        [Display(Name = "ارسل شده")]
        Sent = 1,
        [Display(Name = "پاسخ داده شده")]
        Answered = 2,
        [Display(Name = "بسته شده")]
        Closed = 3,
    }
}
