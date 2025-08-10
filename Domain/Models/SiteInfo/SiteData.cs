namespace Domain.Models.SiteInfo
{
    public class SiteData : BaseEntity<int>
    {
        public string SiteName { get; set; }
        public string? DarkBgLogo { get; set; }
        public string? LightBgLogo { get; set; }
        public string? Phone { get; set; }
        public string? FooterDescription { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? TermsAndCondition { get; set; }
        public string? PrivacyPolicy { get; set; }
        public string? FooterContent { get; set; }
    }
}
