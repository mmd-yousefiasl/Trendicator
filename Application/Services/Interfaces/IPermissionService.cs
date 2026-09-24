using Application.Dtos.Permission.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<List<AdminSidePermissionViewModel>>> GetAllPermissions();
    }
}
