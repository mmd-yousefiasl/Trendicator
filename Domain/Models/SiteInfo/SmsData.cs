namespace Domain.Models.SiteInfo
{
    public class SmsData : BaseEntity<int>
    {
        public string? SmsPanelAPIKey { get; set; }
        public int? SmsPatternIdForLoginCode { get; set; }
    }
}
