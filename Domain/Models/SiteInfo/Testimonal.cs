namespace Domain.Models.SiteInfo
{
    public class Testimonial : BaseEntity<int>
    {
        public string Content { get; set; }
        public string Writer { get; set; }
        public string Position { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
