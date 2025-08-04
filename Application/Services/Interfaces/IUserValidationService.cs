using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IUserValidationService
    {
        Task<Result<string>> GenerateUserValidationCode(string mobile);

        Task<Result> ValidateUserLoginCode(string mobile, string code);
    }
}
