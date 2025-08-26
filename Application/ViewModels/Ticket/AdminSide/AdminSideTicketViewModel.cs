using Domain.Enums.Tickets;

namespace Application.ViewModels.Ticket.AdminSide
{
    public class AdminSideTicketViewModel : CommonViewModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Subject { get; set; }
        public TicketStatusEnum Status { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
