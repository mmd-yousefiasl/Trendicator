namespace Domain.Models.Blogs
{
    public class BlogTag : BaseEntity<int>
    {
        public string Title { get; set; }

        #region Relations
        public ICollection<BlogTagMapping>? Blogs { get; set; }
        #endregion
    }
}
