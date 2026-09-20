using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Role.AdminSide;
using Data.Context;
using Domain.Models.Identity;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class RoleService : IRoleService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Role> _dbSet;
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public RoleService(TrendicatorDbContext context,
            IUserService userService)
        {
            _context = context;
            _dbSet = context.Set<Role>();
            _userService = userService;
        }
        #endregion

        public async Task<bool> CheckUserAccess(int userId, string permission)
        {
            if (userId < 1 || permission is null) return false;

            if (await _userService.IsSuperAdmin(userId))
                return true;

            var roleIds = await _dbSet.Where(role => role.RolePermissionMapping.Any(mapping => mapping.Permission.UniqueName == permission)).Select(n => n.Id).ToListAsync();
            if (!roleIds?.Any() ?? true) return false;

            return await _userService.CheckUserRoleIds(userId, roleIds);
        }

        public async Task<Result<bool>> UserCanAccessThisPermissions(int userId, string permission)
        {
            if (userId < 1 || permission is null) return Result.Failure<bool>(Messages.OperationFailedError);

            if (await _userService.IsSuperAdmin(userId))
                return true;

            var roleIds = await _dbSet.Where(role => role.RolePermissionMapping.Any(mapping => mapping.Permission.UniqueName == permission)).Select(n => n.Id).ToListAsync();
            if (!roleIds?.Any() ?? true) return Result.Failure<bool>(Messages.OperationFailedError);

            return await _userService.CheckUserRoleIds(userId, roleIds);
        }

        public async Task<Result<List<AdminSideRoleViewModel>>> GetAllRolesForUserCRUD()
        {
            var roles = await _dbSet
                .Select(n => new AdminSideRoleViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    UniqueName = n.UniqueName
                }).AsNoTracking().ToListAsync();

            if (!roles?.Any() ?? true) return Result.Failure<List<AdminSideRoleViewModel>>(Messages.OperationFailedError);

            return roles;
        }

        public async Task<Result<AdminSideFilterRolesViewModel>> AdminSideFilterRoles(AdminSideFilterRolesViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterRolesViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Role>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) || n.UniqueName.Contains(filter.Search));
            }

            IQueryable<Role> query = _dbSet;

            query = query.OrderByDescending(n => n.Id);

            foreach (var condition in conditions ?? new FilterConditions<Role>())
            {
                query = query.Where(condition);
            }

            await filter.Paging(query.Select(n => new AdminSideRoleViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                UniqueName = n.UniqueName
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewRole(AdminSideCreateNewRoleViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.UniqueName == model.UniqueName)) return Result.Failure(Messages.DuplicateUniqueNameForRoleError);

            Role role = new Role()
            {
                Title = model.Title,
                UniqueName = model.UniqueName,
                RolePermissionMapping = new List<RolePermissionMapping>()
            };

            foreach (var item in model.PermissionIds)
            {
                role.RolePermissionMapping.Add(new RolePermissionMapping()
                {
                    RoleId = role.Id,
                    PermissionId = item
                });
            }

            await _dbSet.AddAsync(role);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.SuccessfullyDone);
        }

        public async Task<Result> AdminSideSoftDeleteRole(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var role = await _dbSet.FindAsync(id);
            if (role is null) return Result.Failure(Messages.OperationFailedError);

            role.IsDeleted = true;
            role.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(role);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateRoleViewModel>> AdminSideGetRoleForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateRoleViewModel>(Messages.OperationFailedError);

            var role = await _dbSet.Where(n => n.Id == id).Include(n => n.RolePermissionMapping).Select(n => new AdminSideUpdateRoleViewModel()
            {
                Id = n.Id,
                PermissionIds = n.RolePermissionMapping.Select(n => n.PermissionId).ToList(),
                Title = n.Title,
                UniqueName = n.UniqueName
            }).AsNoTracking().FirstOrDefaultAsync();

            if (role is null) return Result.Failure<AdminSideUpdateRoleViewModel>(Messages.OperationFailedError);

            return role;
        }

        public async Task<Result> AdminSideUpdateRole(AdminSideUpdateRoleViewModel model)
        {
            var role = await _dbSet.Include(n => n.RolePermissionMapping).FirstOrDefaultAsync(n => n.Id == model.Id);

            if (role is null) return Result.Failure(Messages.OperationFailedError);


            if (await _dbSet.AnyAsync(n => n.UniqueName == model.UniqueName && n.Id != model.Id))
                return Result.Failure(Messages.DuplicateUniqueNameForRoleError);

            foreach (var permission in role.RolePermissionMapping)
            {
                role.RolePermissionMapping.Remove(permission);
            }

            if (role.UniqueName == "Admin")
                return Result.Failure(Messages.OperationFailedError);

            role.Title = model.Title;
            role.UniqueName = model.UniqueName;
            role.LastUpdatedDate = DateTime.UtcNow;

            foreach (var item in model.PermissionIds ?? new List<int>())
            {
                role.RolePermissionMapping.Add(new RolePermissionMapping
                {
                    RoleId = role.Id,
                    PermissionId = item
                });
            }

            _dbSet.Update(role);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.SuccessfullyDone);
        }
    }
}
