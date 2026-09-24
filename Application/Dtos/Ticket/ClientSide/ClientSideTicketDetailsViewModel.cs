using Application.Dtos;
using Domain.Enums.Tickets;

namespace Application.Dtos.Ticket.ClientSide
{
    public class ClientSideTicketDetailsViewModel : CommonDto
    {
        public DateTime LastUpdatedDate { get; set; }
        public TicketStatusEnum Status { get; set; }
        public string Subject { get; set; }
        public ICollection<ClientSideTicketMessageViewModel> Messages { get; set; }
    }
}
