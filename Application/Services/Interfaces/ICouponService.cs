using Application.Dtos.Coupon.AdminSide;
using Application.Dtos.Coupon.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ICouponService
    {
        #region Admin Side
        Task<Result<AdminSideFilterCouponsViewModel>> AdminSideFilterCoupons(AdminSideFilterCouponsViewModel filter);

        Task<Result> AdminSideSoftDeleteCoupon(int id);

        Task<Result> AdminSideCreateNewCoupon(AdminSideCreateNewCouponViewModel model);

        Task<Result<AdminSideUpdateCouponViewModel>> AdminSideGetCouponFouUpdate(int id);

        Task<Result> AdminSideUpdateCoupon(AdminSideUpdateCouponViewModel model);
        #endregion

        #region Client Side
        Task<Result<List<ClientSideCouponSnapshotDto>>> ClientSideGetCouponSnapshot();

        Task<Result<int>> ClientSideCheckCouponIsValid(string code);
        #endregion
    }
}
