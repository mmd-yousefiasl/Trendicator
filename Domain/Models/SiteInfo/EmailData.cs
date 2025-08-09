namespace Domain.Models.SiteInfo
{
    public class EmailData : BaseEntity<int>
    {
        public string? From { get; set; }
        public string? Password { get; set; }
        public string? EmailDisplayName { get; set; }
        public string? Smtp { get; set; }
        public int? Port { get; set; }
    }
}
