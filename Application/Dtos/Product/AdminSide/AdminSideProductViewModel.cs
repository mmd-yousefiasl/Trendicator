using Domain.Enums.Products;

namespace Application.Dtos.Product.AdminSide
{
    public class AdminSideProductViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public decimal DisplayPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public int ViewsCount { get; set; }
        public ProductTypeEnum Type { get; set; }
    }
}
