namespace Application.Dtos.ProductOption.AdminSide
{
    public class AdminSideProductOptionViewModel : CommonDto
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal? OrderPrice { get; set; }
    }
}
