namespace Domain.Models.SiteInfo
{
    public class Hero : BaseEntity<int>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
    }
}
