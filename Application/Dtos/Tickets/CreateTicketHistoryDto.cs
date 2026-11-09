using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets
{
    public class CreateTicketHistoryDto
    {
        [MaxLength(1000)]
        public string Description { get; set; }
        public int TicketId { get; set; }
    }
}
