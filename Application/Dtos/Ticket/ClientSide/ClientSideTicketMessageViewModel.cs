namespace Application.Dtos.Ticket.ClientSide
{
    public class ClientSideTicketMessageViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string Message { get; set; }
        public bool IsSentByUser { get; set; }
    }
}
