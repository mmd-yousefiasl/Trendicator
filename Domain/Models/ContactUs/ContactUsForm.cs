using Domain.Enums.ContactUs;

namespace Domain.Models.ContactUs
{
    public class ContactUsForm : AuditBaseEntity<int>
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
        public ContactUsFormTypeEnum Type { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
    }
}
