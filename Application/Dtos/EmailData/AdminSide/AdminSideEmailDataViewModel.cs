using Application.Dtos;

namespace Application.Dtos.EmailData.AdminSide
{
    public class AdminSideEmailDataViewModel : CommonDto
    {
        public string From { get; set; }
        public string Password { get; set; }
        public string EmailDisplayName { get; set; }
        public string Smtp { get; set; }
        public int Port { get; set; }
    }
}
