using Application.ViewModels.Permission.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<List<AdminSidePermissionViewModel>>> GetAllPermissions();
    }
}
