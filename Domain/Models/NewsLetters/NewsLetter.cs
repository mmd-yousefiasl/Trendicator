namespace Domain.Models.NewsLetters
{
    public class NewsLetter : AuditBaseEntity<int>
    {
        public string Email { get; set; }
        public string UserIP { get; set; }
        public int? UserId { get; set; }
    }
}
