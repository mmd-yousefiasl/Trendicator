using Application.Dtos;
using Domain.Enums.Tickets;

namespace Application.Dtos.Ticket.AdminSide
{
    public class AdminSideTicketDetailsViewModel : CommonDto
    {
        public DateTime LastUpdatedDate { get; set; }
        public TicketStatusEnum Status { get; set; }
        public string Subject { get; set; }
        public string? UserName { get; set; }
        public ICollection<AdminSideTicketMessageViewModel> Messages { get; set; }
    }
}
