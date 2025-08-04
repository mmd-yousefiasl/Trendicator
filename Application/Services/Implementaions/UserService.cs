using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.User.AdminSide;
using Data.Context;
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
        #endregion

        #region Ctor
        public UserService(IHttpContextAccessor httpContextAccessor,
            TrendicatorDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _dbSet = context.Set<User>();
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

        public async Task<decimal> GetCurrentUserBalance()
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
                BirthDate = n.BirthDate,
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
            user.BirthDate = model.BirthDate;

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
    }
}