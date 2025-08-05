namespace Domain.Models.Faqs
{
    public class Faq : BaseEntity<int>
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
