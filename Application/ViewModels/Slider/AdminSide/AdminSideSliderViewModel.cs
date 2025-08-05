namespace Application.ViewModels.Slider.AdminSide
{
    public class AdminSideSliderViewModel : CommonViewModel
    {
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string ButtonTitle { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
