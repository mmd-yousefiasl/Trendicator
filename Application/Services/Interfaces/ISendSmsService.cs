using Application.ViewModels.SmsData.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISendSmsService
    {
        Task<Result> ClientSideSendLoginCode(ClientSideSendLoginCodeDto dto);
    }
}
