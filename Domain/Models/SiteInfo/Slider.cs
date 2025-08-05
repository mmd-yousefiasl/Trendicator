namespace Domain.Models.SiteInfo
{
    public class Slider : BaseEntity<int>
    {
        public string Banner { get; set; }
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string ButtonTitle { get; set; }
        public string Url { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
