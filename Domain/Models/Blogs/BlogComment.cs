namespace Domain.Models.Blogs
{
    public class BlogComment : AuditBaseEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public bool IsPublished { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }
        public string? Reply { get; set; }

        #region Relations
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        #endregion
    }
}
