using Application.Dtos.SentLetter.AdminSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.NewsLetters;
using Domain.Models.NewsLetters;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SentLetterService : ISentLetterService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<SentLetter> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public SentLetterService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<SentLetter>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterSentLettersViewModel>> AdminSideFilterSentLetters(AdminSideFilterSentLettersViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterSentLettersViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SentLetter>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Subject.Contains(filter.Search) ||
                n.Message.Contains(filter.Search));
            }

            if (filter.IsSucceed is not null)
            {
                conditions.Add(n => n.IsSucceed == filter.IsSucceed);
            }

            IQueryable<SentLetter> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var condition in conditions ?? new FilterConditions<SentLetter>())
            {
                query = query.Where(condition);
            }

            await filter.Paging(query.Select(n => new AdminSideSentLetterViewModel()
            {
                Id = n.Id,
                SentDate = n.CreatedDate,
                Subject = n.Subject,
                IsSucceed = n.IsSucceed,
                Message = n.Message,
                TargetMailCount = n.TargetMailCount
            }));

            return filter;
        }

        public async Task<Result> CreateNewSentLetter(AdminSideCreateNewSentLetterViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            SentLetter letter = new SentLetter()
            {
                Subject = model.Subject,
                Message = model.Message,
                TargetMailCount = model.TargetMailCount,
                IsSucceed = model.IsSucceed,
            };

            await _dbSet.AddAsync(letter);
            await _context.SaveChangesAsync();

            SentLetterCreatedEvent SentLetterCreatedEvent = new SentLetterCreatedEvent(
                letter.Subject,
                letter.Message,
                letter.TargetMailCount,
                letter.IsSucceed);

            await _mediator.PublishEvent(SentLetterCreatedEvent);

            return Result.Success(Messages.SuccessfullyDone);
        }

        public async Task<Result> DeleteSentLetter(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var letter = await _dbSet.FindAsync(id);
            if (letter is null) return Result.Failure(Messages.OperationFailedError);

            letter.LastUpdatedDate = DateTime.UtcNow;
            letter.IsDeleted = true;

            _dbSet.Update(letter);
            await _context.SaveChangesAsync();

            SentLetterDeletedEvent SentLetterDeletedEvent = new SentLetterDeletedEvent(
                letter.Id);

            await _mediator.PublishEvent(SentLetterDeletedEvent);

            return Result.Success(Messages.SuccessfullyDone);
        }
    }
}
