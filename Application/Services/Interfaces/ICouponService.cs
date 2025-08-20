using Application.ViewModels.Coupon.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICouponService
    {
        Task<Result<AdminSideFilterCouponsViewModel>> AdminSideFilterCoupons(AdminSideFilterCouponsViewModel filter);

        Task<Result> SoftDeleteCoupon(int id);

        Task<Result> CreateNewCoupon(AdminSideCreateNewCouponViewModel model);

        Task<Result<AdminSideUpdateCouponViewModel>> GetCouponFouUpdate(int id);

        Task<Result> UpdateCoupon(AdminSideUpdateCouponViewModel model);

        Task<Result> CheckCouponIsValidForUser(string code);

        Task<Result<int>> ClientSideGetCouponPercentByCode(string code);
    }
}
