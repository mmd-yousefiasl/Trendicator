using Domain.Enums.Tickets;
using Domain.Models.Users;

namespace Domain.Models.Tickets
{
    public class Ticket : AuditBaseEntity<int>
    {
        public string Subject { get; set; }
        public TicketStatusEnum Status { get; set; }

        #region Relations
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<TicketMessage> Messages { get; set; }
        #endregion
    }
}
