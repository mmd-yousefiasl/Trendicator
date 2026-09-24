namespace Application.Dtos.SendEmail
{
    public class SendBuckEmailViewModel
    {
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
