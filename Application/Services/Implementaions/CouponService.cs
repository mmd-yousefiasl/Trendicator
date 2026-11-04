using Application.Dtos.Coupon.AdminSide;
using Application.Dtos.Coupon.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        public CouponService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        #region Admin Side
        public async Task<Result<AdminSideFilterCouponsDto>> AdminSideFilterCoupons(AdminSideFilterCouponsDto filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterCouponsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Coupon>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Code.Contains(filter.Search));
            }

            IQueryable<Coupon> query = _context.Coupons;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Coupon>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideCouponDto()
            {
                Id = n.Id,
                Code = n.Code,
                FromDate = n.FromDate,
                Percent = n.Percent,
                ToDate = n.ToDate,
                Title = n.Title,
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewCoupon(AdminSideCreateNewCouponDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            if (model.FromDate > model.ToDate)
                return Result.Failure(Messages.FromDateCantBeGreaterThanToDateError);

            string code = model.Code.NormalizeCouponCode();

            if (await _context.Coupons.AsNoTracking()
                .AnyAsync(n => n.Code == code))
                return Result.Failure(Messages.CouponCodeDuplicateError);

            var coupon = new Coupon()
            {
                FromDate = model.FromDate.ConvertToMiladi(),
                ToDate = model.ToDate.ConvertToMiladi(),
                Code = code,
                Percent = model.Percent,
                Title = model.Title,
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            CouponCreatedEvent CouponCreatedEvent = new CouponCreatedEvent(
            coupon.Title,
            coupon.FromDate,
            coupon.ToDate,
            coupon.Code,
            coupon.Percent);

            await _mediator.PublishEvent(CouponCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateCouponDto>> AdminSideGetCouponFouUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateCouponDto>(Messages.OperationFailedError);

            var coupon = await _context.Coupons
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateCouponDto()
                {
                    Id = n.Id,
                    Code = n.Code,
                    FromDate = n.FromDate.ConvertToShamsi(),
                    ToDate = n.ToDate.ConvertToShamsi(),
                    Percent = n.Percent,
                    Title = n.Title,
                }).FirstOrDefaultAsync();

            if (coupon is null)
                return Result.Failure<AdminSideUpdateCouponDto>(Messages.OperationFailedError);

            return coupon;
        }

        public async Task<Result> AdminSideSoftDeleteCoupon(int id)
        {
            if (id < 1)
                return Result.Failure(Messages.OperationFailedError);

            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon is null)
                return Result.Failure(Messages.OperationFailedError);

            coupon.IsDeleted = true;
            coupon.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            CouponDeletedEvent CouponDeletedEvent = new CouponDeletedEvent(
                coupon.Id);

            await _mediator.PublishEvent(CouponDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideUpdateCoupon(AdminSideUpdateCouponDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            string code = model.Code.NormalizeCouponCode();
            if (await _context.Coupons
                .AsNoTracking().Where(n => n.Id != model.Id && n.Code == code).AnyAsync())
                return Result.Failure(Messages.CouponCodeDuplicateError);

            var coupon = await _context.Coupons.FindAsync(model.Id);

            if (coupon is null)
                return Result.Failure(Messages.OperationFailedError);

            coupon.FromDate = model.FromDate.ConvertToMiladi();
            coupon.ToDate = model.ToDate.ConvertToMiladi();
            coupon.Code = code;
            coupon.Percent = model.Percent;
            coupon.Title = model.Title;

            await _context.SaveChangesAsync();

            CouponUpdatedEvent CouponUpdatedEvent = new CouponUpdatedEvent(
            coupon.Id,
            coupon.Title,
            coupon.FromDate,
            coupon.ToDate,
            coupon.Code,
            coupon.Percent);

            await _mediator.PublishEvent(CouponUpdatedEvent);

            return Result.Success();
        }

        #endregion

        #region Client Side
        public async Task<Result<List<ClientSideCouponSnapshotDto>>> ClientSideGetCouponSnapshot()
        {
            var couponsSnapshot = await _cacheManager.GetAsync(CacheKeys.CouponCacheKey, async () =>
            {
                return await _context.Coupons
                .AsNoTracking().Select(n => new ClientSideCouponSnapshotDto()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Code = n.Code,
                    FromDate = n.FromDate,
                    ToDate = n.ToDate,
                    Percent = n.Percent
                }).ToListAsync();
            });

            if (couponsSnapshot is null || !couponsSnapshot.Any())
                return Result.Failure<List<ClientSideCouponSnapshotDto>>(Messages.OperationFailedError);

            return couponsSnapshot;
        }

        public async Task<Result<int>> ClientSideCheckCouponIsValid(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result.Failure<int>(Messages.InvalidCouponCodeError);

            var couponsResult = await ClientSideGetCouponSnapshot();

            if (couponsResult.IsFailure)
                return Result.Failure<int>(Messages.InvalidCouponCodeError);

            var currentDateTime = DateTime.UtcNow;

            var coupon = couponsResult.Value.FirstOrDefault(c => c.Code == code.NormalizeCouponCode());
            if (coupon is null)
                return Result.Failure<int>(Messages.InvalidCouponCodeError);

            if (coupon.FromDate <= currentDateTime && coupon.ToDate > currentDateTime)
                return coupon.Percent;

            return Result.Failure<int>(Messages.InvalidCouponCodeError);
        }
        #endregion
    }
}