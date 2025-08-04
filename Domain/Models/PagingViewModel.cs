namespace Domain.Models
{
    public class PagingViewModel
    {
        public int Page { get; set; }

        public int StartPage { get; set; }

        public int EndPage { get; set; }

        public int PageCount { get; set; }

        public string ShownEntitesTitle { get; set; }
    }
}
