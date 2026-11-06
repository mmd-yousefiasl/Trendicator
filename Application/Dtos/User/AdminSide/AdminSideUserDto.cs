namespace Application.Dtos.User.AdminSide
{
    public class AdminSideUserDto : CommonDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Mobile { get; set; }
        public bool IsUserBanned { get; set; }
        public decimal Balance { get; set; }
        public bool DeleteRequest { get; set; }
        public DateTime? DeleteRequestDate { get; set; }
    }
}
