namespace Application.ViewModels.SentLetter.AdminSide
{
    public class AdminSideSentLetterViewModel : CommonDto
    {
        public DateTime SentDate { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int TargetMailCount { get; set; }
        public bool IsSucceed { get; set; }
    }
}
