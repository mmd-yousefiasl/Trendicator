using Application.ViewModels.User.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsSuperAdmin(int userId);

        Task<bool> CheckUserRoleIds(int userId, List<int> roleIds);

        Task<Result<AdminSideFilterUsersViewModel>> AdminSideFilterUsers(AdminSideFilterUsersViewModel filter);

        Task<Result> CreateNewUser(AdminSideCreateNewUserViewModel model);

        Task<Result<AdminSideUpdateUserViewModel>> AdminSideGetUserForUpdate(int id);

        Task<Result> UpdateUser(AdminSideUpdateUserViewModel model);

        Task<Result> SoftDeleteUser(int id);

        Task<Result<AdminSideUserDetailsViewModel>> AdminSideGetUserDetails(int id);

        Task<decimal> GetCurrentUserBalance();

        Task<Result<AdminSideUpdateUserProfileViewModel>> AdminSideGetUserProfileForUpdate();

        Task<Result> AdminSideUpdateUserProfile(AdminSideUpdateUserProfileViewModel model);

        Task<Result<string>> AdminSideGetAdminAvatar();
    }
}
