using Domain.Models.Identity;

namespace Domain.Statics
{
    public class PermissionsList
    {
        private static readonly List<Permission> _permissions = new List<Permission>()
        {
            new Permission
            {
                Id = 1,
                ParentId = null,
                Title = "Admin Panel",
                UniqueName = "AdminPanel"
            },
        };

        //Last used id ==> 1
        public static List<Permission> Permissions = _permissions;
    }
}
