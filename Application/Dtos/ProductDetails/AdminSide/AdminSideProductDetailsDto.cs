namespace Application.Dtos.ProductDetails.AdminSide
{
    public class AdminSideProductDetailsDto : CommonDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public int ProductId { get; set; }
    }
}
