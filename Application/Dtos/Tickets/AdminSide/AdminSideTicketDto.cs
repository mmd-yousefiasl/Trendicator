using Domain.Enums.Tickets;

namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideTicketDto : CommonDto
    {
        public string Title { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketPriorityEnum Priority { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
