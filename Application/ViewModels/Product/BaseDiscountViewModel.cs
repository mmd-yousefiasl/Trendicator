namespace Application.ViewModels.Product
{
    public class BaseDiscountViewModel : CommonViewModel
    {
        public bool HaveDiscount { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }
        public int Percent { get; set; }
    }
}
