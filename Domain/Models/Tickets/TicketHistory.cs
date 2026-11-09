namespace Domain.Models.Tickets
{
    public class TicketHistory : AuditBaseEntity<int>
    {
        public string Description { get; set; }

        #region Relations
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int ActorId { get; set; }
        #endregion
    }
}
