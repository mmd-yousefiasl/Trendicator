namespace Application.Dtos.Tickets.AdminSide
{
    public class AdminSideTicketHistoryDto : CommonDto
    {
        public string Description { get; set; }
        public int ActorId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
