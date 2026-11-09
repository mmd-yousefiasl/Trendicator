namespace Application.Dtos.Tickets.ClientSide
{
    public class ClientSideTicketMessageDto : CommonDto
    {
        public string Text { get; set; }
        public string? AttachmentPath { get; set; }
        public bool IsAdminResponse { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
