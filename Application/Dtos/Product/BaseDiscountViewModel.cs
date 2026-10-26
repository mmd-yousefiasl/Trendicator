namespace Application.Dtos.Product
{
    public class BaseDiscountViewModel : CommonDto
    {
        public bool HaveDiscount { get; set; }
        public decimal DisplayPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public int Percent { get; set; }
    }
}
