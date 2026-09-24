using Application.Dtos.Coupon.AdminSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.Coupons;
using Domain.Models.Coupons;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Application.Services.Implementaions
{
    public class CouponService : ICouponService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Coupon> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;

        #endregion

        #region Ctor
        public CouponService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Coupon>();
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterCouponsViewModel>> AdminSideFilterCoupons(AdminSideFilterCouponsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterCouponsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Coupon>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Code.Contains(filter.Search));
            }

            IQueryable<Coupon> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Coupon>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideCouponViewModel()
            {
                Id = n.Id,
                Code = n.Code,
                FromDate = n.FromDate,
                Percent = n.Percent,
                ToDate = n.ToDate,
                Title = n.Title
            }));

            return filter;
        }

        public async Task<Result> CreateNewCoupon(AdminSideCreateNewCouponViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (model.FromDate > model.ToDate) return Result.Failure(Messages.FromDateCantBeGreaterThanToDateError);
            if (await _dbSet.AnyAsync(n => n.Code == model.Code.ToUpper())) return Result.Failure(Messages.CouponCodeDuplicateError);

            var coupon = new Coupon()
            {
                FromDate = model.FromDate.ConvertToMiladi(),
                ToDate = model.ToDate.ConvertToMiladi(),
                Code = model.Code.ToUpper(),
                Percent = model.Percent,
                Title = model.Title,
                Users = model.UserIds?.Select(n => new CouponUserMapping()
                {
                    UserId = n
                }).ToList()
            };

            await _dbSet.AddAsync(coupon);
            await _context.SaveChangesAsync();

            CouponCreatedEvent CouponCreatedEvent = new CouponCreatedEvent(
            coupon.Title,
            coupon.FromDate,
            coupon.ToDate,
            coupon.Code,
            coupon.Percent,
            coupon.Users?.Select(n => n.UserId).ToList() ?? new List<int>());

            await _mediator.PublishEvent(CouponCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateCouponViewModel>> GetCouponFouUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateCouponViewModel>(Messages.OperationFailedError);

            var coupon = await _dbSet.Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateCouponViewModel()
                {
                    Id = n.Id,
                    Code = n.Code,
                    FromDate = n.FromDate.ConvertToShamsi(),
                    ToDate = n.ToDate.ConvertToShamsi(),
                    Percent = n.Percent,
                    Title = n.Title,
                    UserIds = n.Users.Select(n => n.UserId).ToList()
                }).AsNoTracking().FirstOrDefaultAsync();
            if (coupon is null) return Result.Failure<AdminSideUpdateCouponViewModel>(Messages.OperationFailedError);

            return coupon;
        }

        public async Task<Result> SoftDeleteCoupon(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var coupon = await _dbSet.FindAsync(id);
            if (coupon is null) return Result.Failure(Messages.OperationFailedError);

            coupon.IsDeleted = true;
            coupon.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(coupon);
            await _context.SaveChangesAsync();

            CouponDeletedEvent CouponDeletedEvent = new CouponDeletedEvent(
                coupon.Id);

            await _mediator.PublishEvent(CouponDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateCoupon(AdminSideUpdateCouponViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var coupon = await _dbSet.Include(n => n.Users).FirstOrDefaultAsync(n => n.Id == model.Id);
            if (coupon is null) return Result.Failure(Messages.OperationFailedError);

            coupon.FromDate = model.FromDate.ConvertToMiladi();
            coupon.ToDate = model.ToDate.ConvertToMiladi();
            coupon.Code = model.Code;
            coupon.Percent = model.Percent;
            coupon.Title = model.Title;

            var removedUsers = coupon.Users.Select(n => n.UserId).Except(model.UserIds ?? new List<int>()).ToList();
            foreach (var item in removedUsers ?? new List<int>())
            {
                coupon.Users.Remove(coupon.Users.First(n => n.UserId == item));
            }

            var newUsers = model.UserIds?.Except(coupon.Users.Select(n => n.UserId)).ToList();
            foreach (var item in newUsers ?? new List<int>())
            {
                coupon.Users.Add(new CouponUserMapping
                {
                    UserId = item
                });
            }

            _dbSet.Update(coupon);
            await _context.SaveChangesAsync();

            CouponUpdatedEvent CouponUpdatedEvent = new CouponUpdatedEvent(
            coupon.Id,
            coupon.Title,
            coupon.FromDate,
            coupon.ToDate,
            coupon.Code,
            coupon.Percent,
            coupon.Users.Select(n => n.UserId).ToList());

            await _mediator.PublishEvent(CouponUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> CheckCouponIsValidForUser(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result.Failure(Messages.InvalidCouponCodeError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure(Messages.InvalidCouponCodeError);

            var currentDateTime = DateTime.UtcNow;

            bool isValid = await _dbSet.AnyAsync(coupon =>
                coupon.Code == code && coupon.FromDate <= currentDateTime && coupon.ToDate > currentDateTime &&
                (!coupon.Users.Any() || coupon.Users.Any(u => u.UserId == userId)));

            return isValid
                ? Result.Success()
                : Result.Failure(Messages.InvalidCouponCodeError);
        }

        public async Task<Result<int>> ClientSideGetCouponPercentByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return Result.Failure<int>(Messages.OperationFailedError);

            var percent = await _dbSet.Where(n => n.Code == code).Select(n => n.Percent).FirstOrDefaultAsync();
            if (percent < 1) return Result.Failure<int>(Messages.OperationFailedError);

            return percent;
        }
    }
}
