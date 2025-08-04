namespace Domain.Models.Users
{
    public class UserRoleMapping
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        #region Relations

        public User User { get; set; }

        #endregion
    }
}
