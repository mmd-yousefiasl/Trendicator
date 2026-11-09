using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets.ClientSide
{
    public class ClientSideSendTicketMessageDto
    {
        [Required]
        public int TicketId { get; set; }

        [Display(Name = "متن پاسخ")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1500, ErrorMessage = Messages.MaxLengthError)]
        public string MessageText { get; set; } = string.Empty;

        [Display(Name = "فایل پیوست")]
        public IFormFile? Attachment { get; set; }
    }
}
