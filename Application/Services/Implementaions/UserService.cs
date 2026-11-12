using Application.Dtos.SmsData.ClientSide;
using Application.Dtos.User.AdminSide;
using Application.Dtos.User.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Users;
using Domain.Events.Users;
using Domain.Models.Users;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Services.Implementaions
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TrendicatorDbContext _context;
        private readonly ISendSmsService _sendSmsService;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public UserService(IHttpContextAccessor httpContextAccessor,
            TrendicatorDbContext context,
            ISendSmsService sendSmsService,
            IMediatorHandler mediator)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _sendSmsService = sendSmsService;
            _mediator = mediator;
        }
        #endregion

        public async Task<bool> IsSuperAdmin(int userId)
        {
            if (await _context.Users
                .AsNoTracking()
                .AnyAsync(user => user.Id == userId && user.UserRoleMapping.Any(mapping => mapping.RoleId == 1)))
                return true;

            return false;
        }

        public async Task<bool> CheckUserRoleIds(int userId, List<int> roleIds)
        {
            if (await _context.Users
                .AsNoTracking()
                .AnyAsync(user => user.Id == userId && user.UserRoleMapping.Any(mapping => roleIds.Contains(mapping.RoleId))))
                return true;

            return false;
        }

        public async Task<Result<AdminSideFilterUsersDto>> AdminSideFilterUsers(AdminSideFilterUsersDto filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterUsersDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<User>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Mobile.Contains(filter.Search) || n.FirstName.Contains(filter.Search) ||
                n.LastName.Contains(filter.Search));
            }

            if (filter.IsUserBanned is not null)
            {
                conditions.Add(n => n.IsUserBanned == filter.IsUserBanned);
            }

            if (filter.DeleteRequest is not null)
            {
                conditions.Add(n => n.DeleteRequest == filter.DeleteRequest);
            }

            IQueryable<User> query = _context.Users;

            query = query.OrderByDescending(n => n.Id);

            foreach (var condition in conditions ?? new FilterConditions<User>())
            {
                query = query.Where(condition);
            }

            await filter.Paging(query.Select(n => new AdminSideUserDto()
            {
                Id = n.Id,
                Mobile = n.Mobile,
                FirstName = n.FirstName,
                LastName = n.LastName,
                IsUserBanned = n.IsUserBanned,
                Balance = n.Balance,
                DeleteRequest = n.DeleteRequest,
                DeleteRequestDate = n.DeleteRequestDate
            }));

            return filter;
        }

        public async Task<Result> CreateNewUser(AdminSideCreateNewUserDto model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _context.Users
                .AsNoTracking()
                .AnyAsync(n => n.Mobile == model.Mobile))
                return Result.Failure(Messages.ThisPhoneNumberAlreadyRegisteredError);

            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsUserBanned = model.IsUserBanned,
                Mobile = model.Mobile,
                UserRoleMapping = new List<UserRoleMapping>(),
            };

            if (model.RoleIds?.Any() ?? false)
            {
                foreach (var item in model.RoleIds ?? new List<int>())
                {
                    user.UserRoleMapping.Add(new UserRoleMapping()
                    {
                        UserId = user.Id,
                        RoleId = item,
                    });
                }
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateUserDto>> AdminSideGetUserForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateUserDto>(Messages.OperationFailedError);

            var user = await _context.Users
                .AsNoTracking()
                .Where(n => n.Id == id).Select(n => new AdminSideUpdateUserDto()
                {
                    Id = n.Id,
                    FirstName = n.FirstName,
                    LastName = n.LastName,
                    IsUserBanned = n.IsUserBanned,
                    Mobile = n.Mobile,
                    RoleIds = n.UserRoleMapping.Select(n => n.RoleId).ToList()
                }).FirstOrDefaultAsync();

            if (user is null) return Result.Failure<AdminSideUpdateUserDto>(Messages.OperationFailedError);

            return user;
        }

        public async Task<Result> UpdateUser(AdminSideUpdateUserDto model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Users
                .Include(n => n.UserRoleMapping)
                .FirstOrDefaultAsync(n => n.Id == model.Id);

            if (user is null)
                return Result.Failure(Messages.OperationFailedError);

            if (await _context.Users.AsNoTracking().AnyAsync(n => n.Id != model.Id && n.Mobile == model.Mobile))
                return Result.Failure(Messages.ThisPhoneNumberAlreadyRegisteredError);

            user.LastUpdatedDate = DateTime.UtcNow;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsUserBanned = model.IsUserBanned;
            user.Mobile = model.Mobile;

            var removedRoles = user.UserRoleMapping.Select(n => n.RoleId).Except(model.RoleIds ?? new List<int>()).ToList();

            foreach (var item in removedRoles ?? new List<int>())
            {
                user.UserRoleMapping.Remove(user.UserRoleMapping.First(n => n.RoleId == item));
            }
            var newRoles = model.RoleIds?.Except(user.UserRoleMapping.Select(n => n.RoleId)).ToList();
            foreach (var item in newRoles ?? new List<int>())
            {
                user.UserRoleMapping.Add(new UserRoleMapping
                {
                    RoleId = item
                });
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> SoftDeleteUser(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Users.FindAsync(id);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            user.IsDeleted = true;
            user.LastUpdatedDate = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<decimal> GetCurrentUserBalance()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (userId < 1)
                return 0;

            var balance = await _context.Users
                .AsNoTracking()
                .Where(n => n.Id == userId)
                .Select(n => n.Balance)
                .FirstOrDefaultAsync();

            if (balance < 0)
                return 0;

            return balance;
        }

        public async Task<Result<AdminSideUpdateUserProfileDto>> AdminSideGetUserProfileForUpdate()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<AdminSideUpdateUserProfileDto>(Messages.OperationFailedError);

            var user = await _context.Users
                .AsNoTracking()
                .Where(n => n.Id == userId)
                .Select(n => new AdminSideUpdateUserProfileDto()
                {
                    FirstName = n.FirstName,
                    LastName = n.LastName,
                    DeleteRequest = n.DeleteRequest,
                    DeleteRequestDate = n.DeleteRequestDate,
                    Mobile = n.Mobile,
                }).FirstOrDefaultAsync();

            if (user is null) return Result.Failure<AdminSideUpdateUserProfileDto>(Messages.OperationFailedError);

            return user;
        }

        public async Task<Result> AdminSideUpdateUserProfile(AdminSideUpdateUserProfileDto model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Users.FindAsync(userId);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<DateTime>> SendUserLoginCode(ClientSideLoginDto model)
        {
            if (model is null)
                return Result.Failure<DateTime>(Messages.OperationFailedError);

            User? loginUser = await _context.Users.Where(n => n.Mobile == model.Mobile).FirstOrDefaultAsync();
            if (loginUser is null)
            {
                loginUser = new User();

                loginUser.Mobile = model.Mobile;
                loginUser.IsUserBanned = false;

                _context.Users.Add(loginUser);

                try
                {
                    await _context.SaveChangesAsync();

                    UserCreatedEvent UserCreatedEvent = new UserCreatedEvent(loginUser.Mobile);
                    await _mediator.PublishEvent(UserCreatedEvent);
                }
                catch (DbUpdateException ex)
                    when (ex.InnerException is PostgresException pg && pg.SqlState == "23505" && pg.ConstraintName == "IX_Users_Mobile")
                {
                    _context.Entry(loginUser).State = EntityState.Detached;
                    loginUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Mobile == model.Mobile);

                    if (loginUser is null)
                        return Result.Failure<DateTime>(Messages.OperationFailedError);
                }
            }
            else
            {
                if (loginUser.IsUserBanned)
                    return Result.Failure<DateTime>(Messages.OperationFailedError);

                if (loginUser.LockoutEnd.HasValue && loginUser.LockoutEnd.Value > DateTime.UtcNow)
                {
                    var remainingMins = Math.Ceiling((loginUser.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
                    return Result.Failure<DateTime>($"بیش از حد مجاز تلاش کرده‌اید. لطفا {remainingMins} دقیقه دیگر مجددا تلاش کنید.");
                }
                else
                {
                    loginUser.AccessFailedCount = 0;
                    loginUser.LockoutEnd = null;
                    await _context.SaveChangesAsync();
                }
            }

            var validationResult = await GenerateUserValidationCode(model.Mobile);
            if (validationResult.IsFailure)
                return Result.Failure<DateTime>(validationResult.Message!);

            var validation = validationResult.Value;

            if (validation.HasActiveCode == false && !string.IsNullOrWhiteSpace(validation.Code))
            {
                var smsResult = await _sendSmsService.ClientSideSendLoginCode(new ClientSideSendLoginCodeDto()
                {
                    Code = validation.Code,
                    Mobile = loginUser.Mobile
                });
            }

            return validation.CreatedDate;
        }

        public async Task<Result<ClientSideUserDto>> VerifyLoginCode(ClientSideLoginCodeDto model)
        {
            if (model is null) return Result.Failure<ClientSideUserDto>(Messages.OperationFailedError);

            string loginCode = string.Join("", model.LoginCode);

            var validationResult = await ValidateUserLoginCode(model.Mobile, loginCode);

            if (validationResult.IsFailure)
                return Result.Failure<ClientSideUserDto>(validationResult.Message!);

            var user = await _context.Users
                .AsNoTracking()
                .Where(n => n.Mobile == model.Mobile).Select(n => new ClientSideUserDto()
                {
                    Id = n.Id,
                    IsBanned = n.IsUserBanned,
                    Mobile = n.Mobile,
                }).FirstOrDefaultAsync();

            if (user is null)
                return Result.Failure<ClientSideUserDto>(Messages.OperationFailedError);

            UserLoggedInEvent UserLoggedInEvent = new UserLoggedInEvent(user.Id);

            await _mediator.PublishEvent(UserLoggedInEvent);

            return user;
        }

        public async Task<Result<OtpGenerationResultDto>> GenerateUserValidationCode(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return Result.Failure<OtpGenerationResultDto>(Messages.OperationFailedError);

            var existingValidations = await _context.UserValidation
                .Where(uv => uv.Mobile == mobile)
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();

            var latestValidation = existingValidations.FirstOrDefault();

            if (latestValidation != null && DateTime.UtcNow - latestValidation.CreatedDate < TimeSpan.FromMinutes(2))
            {
                return Result.Success(new OtpGenerationResultDto
                {
                    Code = null,
                    CreatedDate = latestValidation.CreatedDate,
                    HasActiveCode = true
                });
            }

            if (existingValidations.Any())
            {
                _context.UserValidation.RemoveRange(existingValidations);
            }

            var code = Generate.UserLoginCode();
            var hashedCode = code.HashPassword(out string salt);
            var nowUtc = DateTime.UtcNow;

            var validation = new UserValidation
            {
                Code = hashedCode,
                Salt = salt,
                Mobile = mobile,
                CreatedDate = nowUtc,
            };

            _context.UserValidation.Add(validation);
            await _context.SaveChangesAsync();

            return Result.Success(new OtpGenerationResultDto
            {
                Code = code,
                CreatedDate = nowUtc,
                HasActiveCode = false
            });
        }

        public async Task<Result> ValidateUserLoginCode(string mobile, string code)
        {
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(code))
                return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Users.Where(u => u.Mobile == mobile).FirstOrDefaultAsync();
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var remainingMins = Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
                return Result.Failure($"حساب شما قفل شده است. {remainingMins} دقیقه دیگر تلاش کنید.");
            }

            var validation = await _context.UserValidation.Where(uv => uv.Mobile == mobile).FirstOrDefaultAsync();
            if (validation is null || validation.CreatedDate.AddMinutes(2) < DateTime.UtcNow)
            {
                return Result.Failure(Messages.LoginCodeExpiredError);
            }

            if (!code.VerifyPassword(validation.Salt, validation.Code))
            {
                user.AccessFailedCount++;

                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddHours(2);
                    user.AccessFailedCount = 0;
                }

                await _context.SaveChangesAsync();
                return Result.Failure(Messages.InvalidLoginCodeError);
            }

            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            _context.UserValidation.Remove(validation);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<List<AdminSideUserAsOptionDto>>> AdminSideGetUsersForCoupon()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(n => n.IsUserBanned == false)
                .Select(n => new AdminSideUserAsOptionDto()
                {
                    Id = n.Id,
                    Mobile = n.Mobile,
                    FullName = n.FirstName + " " + n.LastName
                }).ToListAsync();

            if (!users.Any())
                return Result.Failure<List<AdminSideUserAsOptionDto>>(Messages.OperationFailedError);

            return users;
        }

        public async Task<Result> WithdrawCurrentUserCart(decimal amount, int orderId)
        {
            if (amount < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Users
                .Where(n => n.Id == userId)
                .Include(n => n.Transactions)
                .FirstOrDefaultAsync();
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            if (user.Balance < amount) return Result.Failure(Messages.InSufficentBalanceError);

            user.Balance = user.Balance - amount;

            user.Transactions?.Add(new BalanceTransaction()
            {
                TransId = null,
                IsSuccess = true,
                Amount = amount,
                Type = BalanceTransactionTypeEnum.Debit,
                UserId = user.Id,
                Description = "کاهش موجودی کیف پول جهت تسویه حساب سفارش با شناسه " + orderId
            });

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<int> SiteUsersCount()
        {
            return await _context.Users
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<Result<string>> ClientSideGetCurrentUserFullName()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure<string>(Messages.OperationFailedError);

            var fullName = await _context.Users
                .AsNoTracking()
                .Where(n => n.Id == userId)
                .Select(n => n.FirstName + " " + n.LastName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(fullName))
                return Result.Failure<string>(Messages.OperationFailedError);

            return fullName;
        }

        public async Task<Result> ClientSideAccountDeleteRequest()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var updated = await _context.Users
                 .Where(n => n.Id == userId && !n.DeleteRequest)
                 .ExecuteUpdateAsync(u => u
                 .SetProperty(n => n.DeleteRequest, true)
                 .SetProperty(n => n.DeleteRequestDate, DateTime.UtcNow));

            if (updated != 1)
                return Result.Failure(Messages.OperationFailedError);

            return Result.Success(Messages.DeleteRequestSubmitedSuccessfully);
        }

        public async Task<Result> ClientSideCancelAccountDelete()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var updated = await _context.Users
                 .Where(n => n.Id == userId && n.DeleteRequest)
                 .ExecuteUpdateAsync(u => u
                 .SetProperty(n => n.DeleteRequest, false)
                 .SetProperty(n => n.DeleteRequestDate, (DateTime?)null));

            if (updated != 1)
                return Result.Failure(Messages.OperationFailedError);

            return Result.Success(Messages.DeleteRequestCanceledSuccessfully);
        }
    }
}