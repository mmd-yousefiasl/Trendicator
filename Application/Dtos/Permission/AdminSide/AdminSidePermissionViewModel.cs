using Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Permission.AdminSide
{
    public class AdminSidePermissionViewModel : CommonDto
    {
        public string Title { get; set; }
        public int? ParentId { get; set; }
    }
}
