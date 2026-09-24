using Application.Dtos;

namespace Application.Dtos.Testimonial.AdminSide
{
    public class AdminSideTestimonialViewModel : CommonDto
    {
        public string Writer { get; set; }
        public string Position { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
    }
}
