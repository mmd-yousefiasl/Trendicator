namespace Domain.Models.Blogs
{
    public class BlogCategory : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string MetaDescription { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }

        #region Relations
        public ICollection<Blog>? Blogs { get; set; }
        #endregion
    }
}
