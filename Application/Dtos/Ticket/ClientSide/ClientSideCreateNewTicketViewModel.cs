using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Ticket.ClientSide
{
    public class ClientSideCreateNewTicketViewModel
    {
        [Display(Name = "موضوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Subject { get; set; }

        [Display(Name = "پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Message { get; set; }
    }
}
