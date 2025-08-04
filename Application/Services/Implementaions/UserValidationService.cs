using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.Users;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class UserValidationService : IUserValidationService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<UserValidation> _dbSet;
        #endregion

        #region Ctor
        public UserValidationService(TrendicatorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<UserValidation>();
        }
        #endregion

        public async Task<Result<string>> GenerateUserValidationCode(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return Result.Failure<string>(Messages.OperationFailedError);

            var existingValidations = await _dbSet
                .Where(v => v.PhoneNumber == mobile)
                .OrderByDescending(v => v.CreatedDate) // assuming you have a CreatedAt field
                .ToListAsync();

            var latestValidation = existingValidations.FirstOrDefault();
            if (latestValidation != null &&
                DateTime.UtcNow - latestValidation.CreatedDate < TimeSpan.FromMinutes(2))
            {
                return Result.Failure<string>(Messages.ToManyLoginAttemptError);
            }

            if (existingValidations.Any())
                _dbSet.RemoveRange(existingValidations);

            var code = Generate.UserLoginCode();
            var hashedCode = code.HashPassword(out string salt);

            var validation = new UserValidation
            {
                Code = hashedCode,
                Salt = salt,
                PhoneNumber = mobile,
                CreatedDate = DateTime.UtcNow,
            };

            await _dbSet.AddAsync(validation);
            await _context.SaveChangesAsync();

            return code;
        }

        public async Task<Result> ValidateUserLoginCode(string mobile, string code)
        {
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(code)) return Result.Failure(Messages.OperationFailedError);

            var validation = await _dbSet.Where(n => n.PhoneNumber == mobile)
                .FirstOrDefaultAsync();
            if (validation is null)
                return Result.Failure(Messages.OperationFailedError);

            if (!code.VerifyPassword(validation.Salt, validation.Code)) return Result.Failure(Messages.InvalidLoginCodeError);

            _dbSet.Remove(validation);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
