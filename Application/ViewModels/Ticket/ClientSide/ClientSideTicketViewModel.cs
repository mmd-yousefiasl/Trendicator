using Domain.Enums.Tickets;

namespace Application.ViewModels.Ticket.ClientSide
{
    public class ClientSideTicketViewModel : CommonViewModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Subject { get; set; }
        public TicketStatusEnum Status { get; set; }
    }
}
