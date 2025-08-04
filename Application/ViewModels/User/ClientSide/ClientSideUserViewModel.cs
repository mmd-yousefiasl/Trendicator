namespace Application.ViewModels.User.ClientSide
{
    public class ClientSideUserViewModel : CommonViewModel
    {
        public string Mobile { get; set; }
        public string? Avatar { get; set; }
        public bool IsBanned { get; set; }
    }
}
