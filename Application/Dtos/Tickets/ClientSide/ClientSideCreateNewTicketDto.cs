using Domain.Enums.Tickets;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Tickets.ClientSide
{
    public class ClientSideCreateNewTicketDto
    {
        [Display(Name = "موضوع تیکت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "دپارتمان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public TicketCategoryEnum Category { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public TicketPriorityEnum Priority { get; set; }

        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1500, ErrorMessage = Messages.MaxLengthError)]
        public string MessageText { get; set; } = string.Empty;

        [Display(Name = "فایل پیوست")]
        public IFormFile? Attachment { get; set; }
    }
}
