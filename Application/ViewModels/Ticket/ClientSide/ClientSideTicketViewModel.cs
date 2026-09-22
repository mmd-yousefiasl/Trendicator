using Domain.Enums.Tickets;

namespace Application.ViewModels.Ticket.ClientSide
{
    public class ClientSideTicketViewModel : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Subject { get; set; }
        public TicketStatusEnum Status { get; set; }
    }
}
