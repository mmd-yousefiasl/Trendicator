namespace Application.Dtos.ProductCategory.ClientSide
{
    public class ClientSideProductCategoryViewModel : CommonDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
        public List<ClientSideProductCategoryViewModel>? Categories { get; set; }
    }
}
