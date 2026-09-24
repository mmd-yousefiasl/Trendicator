using Application.Dtos.Permission.AdminSide;
using Application.Services.Interfaces;
using Data.Context;
using Domain.Models.Identity;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class PermissionService : IPermissionService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Permission> _dbSet;
        #endregion

        #region Ctor
        public PermissionService(TrendicatorDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Permission>();
        }
        #endregion

        public async Task<Result<List<AdminSidePermissionViewModel>>> GetAllPermissions()
        {
            var permissions = await _dbSet.AsNoTracking().ToListAsync();

            if (!permissions?.Any() ?? true) return Result.Failure<List<AdminSidePermissionViewModel>>(Messages.OperationFailedError);

            return permissions.Select(n => new AdminSidePermissionViewModel()
            {
                Id = n.Id,
                ParentId = n.ParentId,
                Title = n.Title
            }).ToList();
        }
    }
}
