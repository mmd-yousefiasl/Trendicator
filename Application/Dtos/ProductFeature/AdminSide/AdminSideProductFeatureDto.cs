namespace Application.Dtos.ProductFeature.AdminSide
{
    public class AdminSideProductFeatureDto : CommonDto
    {
        public string Text { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public int ProductId { get; set; }
    }
}
