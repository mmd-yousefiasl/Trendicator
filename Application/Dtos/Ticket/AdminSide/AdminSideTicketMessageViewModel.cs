namespace Application.Dtos.Ticket.AdminSide
{
    public class AdminSideTicketMessageViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string Message { get; set; }
        public bool IsSentByUser { get; set; }
    }
}
