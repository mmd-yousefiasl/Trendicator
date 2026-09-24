using Application.Dtos.SendEmail;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISendEmailService
    {
        Task<Result> SendBuckEmail(SendBuckEmailViewModel model);
    }
}
