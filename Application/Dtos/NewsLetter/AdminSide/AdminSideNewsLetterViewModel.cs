using Application.Dtos;

namespace Application.Dtos.NewsLetter.AdminSide
{
    public class AdminSideNewsLetterViewModel : CommonDto
    {
        public string Email { get; set; }
        public string UserIP { get; set; }
    }
}
