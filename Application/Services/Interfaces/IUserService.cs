using Application.ViewModels.User.AdminSide;
using Application.ViewModels.User.ClientSide;
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

        Task<long> GetCurrentUserBalance();

        Task<Result<AdminSideUpdateUserProfileViewModel>> AdminSideGetUserProfileForUpdate();

        Task<Result> AdminSideUpdateUserProfile(AdminSideUpdateUserProfileViewModel model);

        Task<Result<string>> AdminSideGetAdminAvatar();

        Task<Result<OtpGenerationResultDto>> GenerateUserValidationCode(string mobile);

        Task<Result> ValidateUserLoginCode(string mobile, string code);

        Task<Result<DateTime>> SendUserLoginCode(ClientSideLoginDto model);

        Task<Result<ClientSideUserDto>> VerifyLoginCode(ClientSideLoginCodeDto model);

        Task<Result<List<AdminSideUserAsOptionViewModel>>> AdminSideGetUsersForCoupon();

        Task<Result> WithdrawCurrentUserCart(long amount, int orderId);

        Task<int> SiteUsersCount();

        Task<Result<DateTime>> GetMySignInDate();
    }
}
