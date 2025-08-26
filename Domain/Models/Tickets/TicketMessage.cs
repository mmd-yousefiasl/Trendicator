namespace Domain.Models.Tickets
{
    public class TicketMessage : AuditBaseEntity<int>
    {
        public string Message { get; set; }
        public bool IsSentByUser { get; set; }

        #region Relations
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        #endregion
    }
}
