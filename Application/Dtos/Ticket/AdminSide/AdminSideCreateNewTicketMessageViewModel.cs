using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Ticket.AdminSide
{
    public class AdminSideCreateNewTicketMessageViewModel
    {
        public int TicketId { get; set; }

        [Display(Name = "پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Message { get; set; }
    }
}
