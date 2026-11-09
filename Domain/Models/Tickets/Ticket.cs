using Domain.Enums.Tickets;

namespace Domain.Models.Tickets
{
    public class Ticket : AuditBaseEntity<int>
    {
        public string Title { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketPriorityEnum Priority { get; set; }
        public TicketStatusEnum Status { get; set; }

        #region Relations
        public int UserId { get; set; }

        public ICollection<TicketMessage> Messages { get; set; }
        public ICollection<TicketHistory> TicketHistories { get; set; }
        #endregion
    }
}
