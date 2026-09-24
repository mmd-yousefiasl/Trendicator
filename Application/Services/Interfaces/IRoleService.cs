using Application.Dtos.Role.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Result<bool>> UserCanAccessThisPermissions(int userId, string permission);

        Task<bool> CheckUserAccess(int userId, string permission);

        Task<Result<List<AdminSideRoleViewModel>>> GetAllRolesForUserCRUD();

        Task<Result<AdminSideFilterRolesViewModel>> AdminSideFilterRoles(AdminSideFilterRolesViewModel filter);

        Task<Result> AdminSideCreateNewRole(AdminSideCreateNewRoleViewModel model);

        Task<Result> AdminSideSoftDeleteRole(int id);

        Task<Result<AdminSideUpdateRoleViewModel>> AdminSideGetRoleForUpdate(int id);

        Task<Result> AdminSideUpdateRole(AdminSideUpdateRoleViewModel model);
    }
}
