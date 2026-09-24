using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Ticket.ClientSide
{
    public class ClientSideCreateNewTicketMessageViewModel
    {
        public int TicketId { get; set; }

        [Display(Name = "پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Message { get; set; }
    }
}
