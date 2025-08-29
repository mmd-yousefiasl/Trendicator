using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.User.AdminSide;
using Application.ViewModels.User.ClientSide;
using Data.Context;
using Domain.Enums.Users;
using Domain.Models.Users;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TrendicatorDbContext _context;
        private protected DbSet<User> _dbSet;
        private readonly IUserValidationService _userValidationService;
        #endregion

        #region Ctor
        public UserService(IHttpContextAccessor httpContextAccessor,
            TrendicatorDbContext context,
            IUserValidationService userValidationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _dbSet = context.Set<User>();
            _userValidationService = userValidationService;
        }
        #endregion

        public async Task<bool> IsSuperAdmin(int userId)
        {
            if (await _dbSet.AnyAsync(user => user.Id == userId && user.UserRoleMapping.Any(mapping => mapping.RoleId == 1)))
                return true;

            return false;
        }

        public async Task<bool> CheckUserRoleIds(int userId, List<int> roleIds)
        {
            if (await _dbSet.AnyAsync(user => user.Id == userId && user.UserRoleMapping.Any(mapping => roleIds.Contains(mapping.RoleId))))
                return true;

            return false;
        }

        public async Task<Result<AdminSideFilterUsersViewModel>> AdminSideFilterUsers(AdminSideFilterUsersViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterUsersViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<User>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.PhoneNumber.Contains(filter.Search) || n.FirstName.Contains(filter.Search) ||
                n.LastName.Contains(filter.Search));
            }

            if (filter.IsUserBanned is not null)
            {
                conditions.Add(n => n.IsUserBanned == filter.IsUserBanned);
            }

            IQueryable<User> query = _dbSet;

            query = query.OrderByDescending(n => n.Id);

            foreach (var condition in conditions ?? new FilterConditions<User>())
            {
                query = query.Where(condition);
            }

            await filter.Paging(query.Select(n => new AdminSideUserViewModel()
            {
                Id = n.Id,
                PhoneNumber = n.PhoneNumber,
                FirstName = n.FirstName,
                LastName = n.LastName,
                IsUserBanned = n.IsUserBanned,
                Balance = n.Balance
            }));

            return filter;
        }

        public async Task<Result> CreateNewUser(AdminSideCreateNewUserViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.PhoneNumber == model.PhoneNumber)) return Result.Failure(Messages.ThisPhoneNumberAlreadyRegisteredError);

            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsUserBanned = model.IsUserBanned,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate,
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

            if (model.AvatarFile is not null)
            {
                if (model.AvatarFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                if (model.AvatarFile.IsImage() == false) return Result.Failure(Messages.InvalidFileFormatError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AvatarFile.FileName);

                model.AvatarFile.UplaodFileToServer(fileName, SiteTools.UserAvatarFile);

                user.Avatar = fileName;
            }
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateUserViewModel>> AdminSideGetUserForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateUserViewModel>(Messages.OperationFailedError);

            var user = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateUserViewModel()
            {
                Id = n.Id,
                AvatarFileName = n.Avatar,
                BirthDate = n.BirthDate,
                Email = n.Email,
                FirstName = n.FirstName,
                LastName = n.LastName,
                IsUserBanned = n.IsUserBanned,
                PhoneNumber = n.PhoneNumber,
                RoleIds = n.UserRoleMapping.Select(n => n.RoleId).ToList()
            }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Result.Failure<AdminSideUpdateUserViewModel>(Messages.OperationFailedError);

            return user;
        }

        public async Task<Result> UpdateUser(AdminSideUpdateUserViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var user = await _dbSet.Include(n => n.UserRoleMapping).FirstOrDefaultAsync(n => n.Id == model.Id);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            user.LastUpdatedDate = DateTime.Now;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsUserBanned = model.IsUserBanned;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;

            if (await _dbSet.AnyAsync(n => n.Id != model.Id && n.PhoneNumber == model.PhoneNumber)) return Result.Failure(Messages.ThisPhoneNumberAlreadyRegisteredError);

            if (model.AvatarFile is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AvatarFile.FileName);

                user.Avatar?.RemoveFileFromServer(SiteTools.UserAvatarFile);

                model.AvatarFile.UplaodFileToServer(fileName, SiteTools.UserAvatarFile);

                user.Avatar = fileName;
            }

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

            _dbSet.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> SoftDeleteUser(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _dbSet.FindAsync(id);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            user.IsDeleted = true;
            user.LastUpdatedDate = DateTime.Now;

            user.Avatar?.RemoveFileFromServer(SiteTools.UserAvatarFile);

            _dbSet.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideUserDetailsViewModel>> AdminSideGetUserDetails(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUserDetailsViewModel>(Messages.OperationFailedError);

            var user = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUserDetailsViewModel()
            {
                Avatar = n.Avatar,
                BirthDate = n.BirthDate,
                Email = n.Email,
                FirstName = n.FirstName,
                IsUserBanned = n.IsUserBanned,
                LastName = n.LastName,
                PhoneNumber = n.PhoneNumber
            }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Result.Failure<AdminSideUserDetailsViewModel>(Messages.OperationFailedError);

            return user;
        }

        public async Task<long> GetCurrentUserBalance()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return 0;

            var balance = await _dbSet.Where(n => n.Id == userId).Select(n => n.Balance).FirstOrDefaultAsync();
            if (balance < 0) return 0;

            return balance;
        }

        public async Task<Result<AdminSideUpdateUserProfileViewModel>> AdminSideGetUserProfileForUpdate()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<AdminSideUpdateUserProfileViewModel>(Messages.OperationFailedError);

            var user = await _dbSet.Where(n => n.Id == userId).Select(n => new AdminSideUpdateUserProfileViewModel()
            {
                AvatarFileName = n.Avatar,
                FirstName = n.FirstName,
                LastName = n.LastName,
                BirthDate = n.BirthDate.ConvertToShamsi(),
                Email = n.Email,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Result.Failure<AdminSideUpdateUserProfileViewModel>(Messages.OperationFailedError);

            return user;
        }

        public async Task<Result> AdminSideUpdateUserProfile(AdminSideUpdateUserProfileViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _dbSet.FindAsync(userId);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate?.ConvertToMiladi();
            user.Email = model.Email;

            if (model.AvatarFile is not null)
            {
                if (!model.AvatarFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.AvatarFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AvatarFile.FileName);
                model.AvatarFile.UplaodFileToServer(fileName, SiteTools.UserAvatarFile);
                user.Avatar?.RemoveFileFromServer(SiteTools.UserAvatarFile);

                user.Avatar = fileName;
            }

            _dbSet.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<string>> AdminSideGetAdminAvatar()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<string>(Messages.OperationFailedError);

            var userAvatar = await _dbSet.Where(n => n.Id == userId).Select(n => n.Avatar).AsNoTracking().FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(userAvatar)) return Result.Failure<string>(Messages.OperationFailedError);

            return userAvatar;
        }

        public async Task<Result<string>> SendUserLoginCode(ClientSideLoginViewModel model)
        {
            if (model is null) return Result.Failure<string>(Messages.OperationFailedError);

            User loginUser = await _dbSet.Where(n => n.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync();
            if (loginUser is null)
            {
                loginUser = new User();

                loginUser.PhoneNumber = model.PhoneNumber;
                loginUser.IsUserBanned = false;

                await _dbSet.AddAsync(loginUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (loginUser.IsUserBanned) return Result.Failure<string>(Messages.OperationFailedError);
            }

            var validationResult = await _userValidationService.GenerateUserValidationCode(model.PhoneNumber);
            if (validationResult.IsFailure) return Result.Failure<string>(validationResult.Message);

            Console.WriteLine("Login Code: " + validationResult.Value);

            //await _smsServiec.SendLoginCode(validationResult.Value, loginUser.Mobile);

            return validationResult.Value;
        }

        public async Task<Result<ClientSideUserViewModel>> VerifyLoginCode(ClientSideLoginCodeViewModel model)
        {
            if (model is null) return Result.Failure<ClientSideUserViewModel>(Messages.OperationFailedError);

            var validationResult = await _userValidationService.ValidateUserLoginCode(model.PhoneNumber, model.LoginCode);
            if (validationResult.IsFailure) return Result.Failure<ClientSideUserViewModel>(validationResult.Message);

            var user = await _dbSet.Where(n => n.PhoneNumber == model.PhoneNumber).Select(n => new ClientSideUserViewModel()
            {
                Id = n.Id,
                Avatar = n.Avatar,
                IsBanned = n.IsUserBanned,
                Mobile = n.PhoneNumber,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Result.Failure<ClientSideUserViewModel>(Messages.OperationFailedError);

            return user;
        }

        public async Task<Result<List<AdminSideUserAsOptionViewModel>>> AdminSideGetUsersForCoupon()
        {
            var users = await _dbSet.Where(n => n.IsUserBanned == false).Select(n => new AdminSideUserAsOptionViewModel()
            {
                Id = n.Id,
                PhoneNumber = n.PhoneNumber,
                FullName = n.FirstName + " " + n.LastName
            }).AsNoTracking().ToListAsync();
            if (!users.Any()) return Result.Failure<List<AdminSideUserAsOptionViewModel>>(Messages.OperationFailedError);

            return users;
        }

        public async Task<Result> WithdrawCurrentUserCart(long amount, int orderId)
        {
            if (amount < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var user = await _dbSet.Where(n => n.Id == userId)
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

            _dbSet.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<int> SiteUsersCount()
        {
            return await _dbSet.CountAsync();
        }
    }
}