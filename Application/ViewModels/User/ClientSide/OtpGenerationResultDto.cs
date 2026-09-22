namespace Application.ViewModels.User.ClientSide
{
    public class OtpGenerationResultDto
    {
        public string? Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasActiveCode { get; set; }
    }
}
