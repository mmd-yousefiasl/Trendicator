using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideFilterTicketHistoryDto : BasePaging<AdminSideTicketHistoryDto>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        public int TicketId { get; set; }
    }
}
