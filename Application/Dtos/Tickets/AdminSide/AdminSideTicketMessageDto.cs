namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideTicketMessageDto : CommonDto
    {
        public string Text { get; set; }
        public string? AttachmentPath { get; set; }
        public bool IsAdminResponse { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
