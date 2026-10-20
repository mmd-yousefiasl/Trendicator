namespace Domain.Models.Blogs
{
    public class BlogView : BaseEntity<int>
    {
        public string VisitorId { get; set; }
        public int? UserId { get; set; }
        public string UserIP { get; set; }

        #region Relations
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
        #endregion
    }
}
