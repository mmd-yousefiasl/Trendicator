namespace Application.ViewModels.Testimonial.AdminSide
{
    public class AdminSideTestimonialViewModel : CommonViewModel
    {
        public string Writer { get; set; }
        public string Position { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
