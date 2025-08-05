namespace Domain.Models.Blogs
{
    public class BlogView : BaseEntity<int>
    {
        public string UserIP { get; set; }
        public int? UserId { get; set; }

        #region Relations
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
        #endregion
    }
}
