namespace Domain.Models.ContactUs
{
    public class ContactUsForm : BaseEntity<int>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
    }
}
