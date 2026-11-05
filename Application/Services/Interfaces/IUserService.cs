using Application.Dtos.User.AdminSide;
using Application.Dtos.User.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsSuperAdmin(int userId);

        Task<bool> CheckUserRoleIds(int userId, List<int> roleIds);

        Task<Result<AdminSideFilterUsersDto>> AdminSideFilterUsers(AdminSideFilterUsersDto filter);

        Task<Result> CreateNewUser(AdminSideCreateNewUserDto model);

        Task<Result<AdminSideUpdateUserDto>> AdminSideGetUserForUpdate(int id);

        Task<Result> UpdateUser(AdminSideUpdateUserDto model);

        Task<Result> SoftDeleteUser(int id);

        Task<Result<AdminSideUserDetailsDto>> AdminSideGetUserDetails(int id);

        Task<decimal> GetCurrentUserBalance();

        Task<Result<AdminSideUpdateUserProfileDto>> AdminSideGetUserProfileForUpdate();

        Task<Result> AdminSideUpdateUserProfile(AdminSideUpdateUserProfileDto model);

        Task<Result<OtpGenerationResultDto>> GenerateUserValidationCode(string mobile);

        Task<Result> ValidateUserLoginCode(string mobile, string code);

        Task<Result<DateTime>> SendUserLoginCode(ClientSideLoginDto model);

        Task<Result<ClientSideUserDto>> VerifyLoginCode(ClientSideLoginCodeDto model);

        Task<Result<List<AdminSideUserAsOptionDto>>> AdminSideGetUsersForCoupon();

        Task<Result> WithdrawCurrentUserCart(decimal amount, int orderId);

        Task<int> SiteUsersCount();

        Task<Result<DateTime>> GetMySignInDate();
    }
}
