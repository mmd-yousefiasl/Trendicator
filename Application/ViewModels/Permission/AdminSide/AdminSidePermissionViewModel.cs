using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Permission.AdminSide
{
    public class AdminSidePermissionViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public int? ParentId { get; set; }
    }
}
