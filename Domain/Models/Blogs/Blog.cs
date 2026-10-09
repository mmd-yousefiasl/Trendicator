namespace Domain.Models.Blogs
{
    public class Blog : AuditBaseEntity<int>
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public string Writer { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public int ReadingTimeInMinutes { get; set; }

        #region Relations
        public int BlogCategoryId { get; set; }
        public BlogCategory Category { get; set; }
        public ICollection<BlogView>? Views { get; set; }
        #endregion
    }
}
