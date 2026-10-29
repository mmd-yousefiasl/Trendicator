namespace Domain.Models.Products
{
    public class ProductView : BaseEntity<int>
    {
        public string VisitorId { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
