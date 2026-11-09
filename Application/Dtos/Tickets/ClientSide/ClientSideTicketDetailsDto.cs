using Domain.Enums.Tickets;

namespace Application.Dtos.Tickets.ClientSide
{
    public class ClientSideTicketDetailsDto : CommonDto
    {
        public string Title { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ClientSideTicketMessageDto> Messages { get; set; }
    }
}
