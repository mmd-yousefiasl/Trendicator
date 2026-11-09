using Domain.Enums.Tickets;

namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideTicketDetailsDto : CommonDto
    {
        public string Title { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<AdminSideTicketMessageDto> Messages { get; set; }
    }
}
