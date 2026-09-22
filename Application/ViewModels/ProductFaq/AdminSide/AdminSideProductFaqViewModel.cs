namespace Application.ViewModels.ProductFaq.AdminSide
{
    public class AdminSideProductFaqViewModel : CommonDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public int ProductId { get; set; }
    }
}
