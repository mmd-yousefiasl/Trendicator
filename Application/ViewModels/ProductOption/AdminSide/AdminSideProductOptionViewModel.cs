namespace Application.ViewModels.ProductOption.AdminSide
{
    public class AdminSideProductOptionViewModel : CommonDto
    {
        public string Title { get; set; }
        public long Price { get; set; }
        public long? OrderPrice { get; set; }
    }
}
