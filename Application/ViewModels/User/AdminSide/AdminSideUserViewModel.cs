namespace Application.ViewModels.User.AdminSide
{
    public class AdminSideUserViewModel : CommonViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsUserBanned { get; set; }
        public decimal Balance { get; set; }
    }
}
