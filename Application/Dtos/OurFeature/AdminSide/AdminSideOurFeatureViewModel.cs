using Application.Dtos;

namespace Application.Dtos.OurFeature.AdminSide
{
    public class AdminSideOurFeatureViewModel : CommonDto
    {
        public string Title { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
