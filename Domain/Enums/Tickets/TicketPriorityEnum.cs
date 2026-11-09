using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Tickets
{
    public enum TicketPriorityEnum
    {
        [Display(Name = "کم")]
        Low = 1,

        [Display(Name = "متوسط")]
        Medium = 2,

        [Display(Name = "بالا")]
        High = 3,

        [Display(Name = "خیلی فوری (بحرانی)")]
        Critical = 4
    }
}
