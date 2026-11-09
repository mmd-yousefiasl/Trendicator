using Domain.Enums.Tickets;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideFilterTicketsDto : BasePaging<AdminSideTicketDto>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "دسته بندی")]
        public TicketCategoryEnum? Category { get; set; }

        [Display(Name = "اولویت")]
        public TicketPriorityEnum? Priority { get; set; }

        [Display(Name = "وضعیت")]
        public TicketStatusEnum? Status { get; set; }
    }
}
