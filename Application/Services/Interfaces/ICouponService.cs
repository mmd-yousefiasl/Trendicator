using Application.Dtos.Coupon.AdminSide;
using Application.Dtos.Coupon.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICouponService
    {
        #region Admin Side
        Task<Result<AdminSideFilterCouponsDto>> AdminSideFilterCoupons(AdminSideFilterCouponsDto filter);

        Task<Result> AdminSideSoftDeleteCoupon(int id);

        Task<Result> AdminSideCreateNewCoupon(AdminSideCreateNewCouponDto model);

        Task<Result<AdminSideUpdateCouponDto>> AdminSideGetCouponFouUpdate(int id);

        Task<Result> AdminSideUpdateCoupon(AdminSideUpdateCouponDto model);
        #endregion

        #region Client Side
        Task<Result<List<ClientSideCouponSnapshotDto>>> ClientSideGetCouponSnapshot();

        Task<Result<int>> ClientSideCheckCouponIsValid(string code);
        #endregion
    }
}
