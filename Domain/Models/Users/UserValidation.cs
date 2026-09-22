namespace Domain.Models.Users
{
    public class UserValidation : AuditBaseEntity<int>
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string Salt { get; set; }
    }
}
