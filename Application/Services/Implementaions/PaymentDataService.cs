using Application.Dtos.PaymentData;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class PaymentDataService : IPaymentDataService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public PaymentDataService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideUpdatePaymentDataViewModel>> AdminSideGetPaymentDataForUpdate()
        {
            var data = await _context.PaymentData
                .AsNoTracking()
                .Where(n => n.Id == 1)
                .Select(n => new AdminSideUpdatePaymentDataViewModel()
                {
                    PaymentGatewayPin = n.PaymentGatewayPin,
                }).FirstOrDefaultAsync();

            if (data is null)
                return Result.Failure<AdminSideUpdatePaymentDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result> AdminSideUpdatePaymentData(AdminSideUpdatePaymentDataViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var data = await _context.PaymentData.FindAsync(1);
            if (data is null) return Result.Failure(Messages.OperationFailedError);

            data.PaymentGatewayPin = model.PaymentGatewayPin;

            _context.PaymentData.Update(data);
            await _context.SaveChangesAsync();

            PaymentDataUpdatedEvent PaymentDataUpdatedEvent = new PaymentDataUpdatedEvent(
                data.Id,
                data.PaymentGatewayPin);

            await _mediator.PublishEvent(PaymentDataUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<string>> GetPin()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.PaymentDataCacheKey, async () =>
            {
                return await _context.PaymentData
                .AsNoTracking()
                .Where(n => n.Id == 1)
                .FirstOrDefaultAsync();
            });

            if (data is null)
                return Result.Failure<string>(Messages.OperationFailedError);

            if (string.IsNullOrWhiteSpace(data.PaymentGatewayPin))
                return Result.Failure<string>(Messages.OperationFailedError);

            return data.PaymentGatewayPin;
        }
    }
}
