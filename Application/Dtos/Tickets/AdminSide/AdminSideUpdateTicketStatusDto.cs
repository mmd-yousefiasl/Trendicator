using Domain.Enums.Tickets;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideUpdateTicketStatusDto : CommonDto
    {
        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public TicketStatusEnum Status { get; set; }
    }
}
