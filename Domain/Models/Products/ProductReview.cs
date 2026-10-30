namespace Domain.Models.Products
{
    public class ProductReview : AuditBaseEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Review { get; set; }
        public int Stars { get; set; }
        public bool IsPublished { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
        public string? Reply { get; set; }

        #region Relations

        public int ProductId { get; set; }
        public Product Product { get; set; }

        #endregion
    }
}
