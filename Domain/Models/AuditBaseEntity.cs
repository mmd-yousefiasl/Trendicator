namespace Domain.Models
{
    public class AuditBaseEntity<T> : BaseEntity<T> where T : IEquatable<T>
    {
        public AuditBaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
        }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
