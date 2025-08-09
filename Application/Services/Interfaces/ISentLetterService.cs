using Application.ViewModels.SentLetter.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISentLetterService
    {
        Task<Result<AdminSideFilterSentLettersViewModel>> AdminSideFilterSentLetters(AdminSideFilterSentLettersViewModel filter);

        Task<Result> DeleteSentLetter(int id);

        Task<Result> CreateNewSentLetter(AdminSideCreateNewSentLetterViewModel model);
    }
}
