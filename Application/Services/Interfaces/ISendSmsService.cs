using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISendSmsService
    {
        Task<Result> SendLoginCode(string code, string mobile);
    }
}
