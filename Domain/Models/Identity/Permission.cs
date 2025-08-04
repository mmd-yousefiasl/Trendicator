namespace Domain.Models.Identity
{
    public class Permission : BaseEntity<int>
    {
        public string Title { get; set; }
        public string UniqueName { get; set; }
        public int? ParentId { get; set; }

        #region Relations

        public ICollection<RolePermissionMapping>? RolePermissionMapping { get; set; }

        #endregion
    }
}
