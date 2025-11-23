using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IRecaptchaService
    {
        Task<Result> VerifyRecaptchaV2(string response);
    }
}
