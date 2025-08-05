namespace Domain.Models.Blogs
{
    public class BlogTagMapping
    {
        public int BlogId { get; set; }
        public int TagId { get; set; }

        #region Relations
        public Blog Blog { get; set; }
        public BlogTag Tag { get; set; }
        #endregion
    }
}
