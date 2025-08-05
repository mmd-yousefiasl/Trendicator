namespace Domain.Models.SiteInfo
{
    public class OurFeature : BaseEntity<int>
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
