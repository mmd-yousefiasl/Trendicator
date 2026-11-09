using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Tickets
{
    public enum TicketCategoryEnum
    {
        [Display(Name = "پشتیبانی فنی")]
        Technical = 1,

        [Display(Name = "امور مالی و پرداخت")]
        Financial = 2,

        [Display(Name = "گزارش باگ و خرابی")]
        BugReport = 3,

        [Display(Name = "سوالات عمومی و پیش از خرید")]
        General = 4,

        [Display(Name = "پیشنهادات و انتقادات")]
        Feedback = 5
    }
}
