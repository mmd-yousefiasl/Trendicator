namespace Domain.Models.Identity
{
    public class Role : AuditBaseEntity<int>
    {
        public string Title { get; set; }
        public string UniqueName { get; set; }

        #region Relations

        public ICollection<RolePermissionMapping> RolePermissionMapping { get; set; }

        #endregion
    }
}
