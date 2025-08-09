namespace Domain.Models.NewsLetters
{
    public class SentLetter : AuditBaseEntity<int>
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public int TargetMailCount { get; set; }
        public bool IsSucceed { get; set; }
    }
}
