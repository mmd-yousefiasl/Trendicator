using Domain.Enums.Tickets;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Ticket.AdminSide
{
    public class AdminSideFilterTicketsViewModel : BasePaging<AdminSideTicketViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت")]
        public TicketStatusEnum? Status { get; set; }
    }
}
