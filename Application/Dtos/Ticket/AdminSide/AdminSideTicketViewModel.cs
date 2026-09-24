using Application.Dtos;
using Domain.Enums.Tickets;

namespace Application.Dtos.Ticket.AdminSide
{
    public class AdminSideTicketViewModel : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Subject { get; set; }
        public TicketStatusEnum Status { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
