namespace Domain.Models.Tickets
{
    public class TicketMessage : AuditBaseEntity<int>
    {
        public bool IsAdminResponse { get; set; }
        public string Text { get; set; }
        public string? AttachmentPath { get; set; }

        #region Relations
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int SenderId { get; set; }
        #endregion
    }
}
