using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Permission.AdminSide
{
    public class AdminSidePermissionViewModel : CommonDto
    {
        public string Title { get; set; }
        public int? ParentId { get; set; }
    }
}
