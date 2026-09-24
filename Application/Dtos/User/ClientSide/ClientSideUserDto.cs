using Application.Dtos;

namespace Application.Dtos.User.ClientSide
{
    public class ClientSideUserDto : CommonDto
    {
        public string? FirstName { get; set; }
        public string Mobile { get; set; }
        public bool IsBanned { get; set; }
    }
}
