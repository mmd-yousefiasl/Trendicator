using Application.Dtos;

namespace Application.Dtos.Faq.AdminSide
{
    public class AdminSideFaqViewModel : CommonDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
