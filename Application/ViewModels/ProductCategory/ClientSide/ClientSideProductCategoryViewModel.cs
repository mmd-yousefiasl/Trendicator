namespace Application.ViewModels.ProductCategory.ClientSide
{
    public class ClientSideProductCategoryViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public List<ClientSideProductCategoryViewModel>? Categories { get; set; }
    }
}
