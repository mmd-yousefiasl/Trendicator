namespace Application.ViewModels.ProductCompare.ClientSide
{
    public class ClientSideProductCompareViewModel : CommonViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public long Price { get; set; }
        public string FileName { get; set; }
        public string CategoryTitle { get; set; }
        public string ShortDescription { get; set; }
        public bool IsAvailable { get; set; }
    }
}
