namespace Application.ViewModels.Faq.AdminSide
{
    public class AdminSideFaqViewModel : CommonViewModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
