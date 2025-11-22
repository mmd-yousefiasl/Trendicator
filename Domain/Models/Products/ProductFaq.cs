namespace Domain.Models.Products
{
    public class ProductFaq : BaseEntity<int>
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
