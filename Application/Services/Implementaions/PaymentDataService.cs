using Application.Mediator;
using Application.Services.Interfaces;
using Application.ViewModels.PaymentData;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class PaymentDataService : IPaymentDataService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<PaymentData> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public PaymentDataService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<PaymentData>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideUpdatePaymentDataViewModel>> AdminSideGetPaymentDataForUpdate()
        {
            var data = await _dbSet.Where(n => n.Id == 1).Select(n => new AdminSideUpdatePaymentDataViewModel()
            {
                Tax = n.Tax ?? 0,
                PaymentGatewayPin = n.PaymentGatewayPin,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (data is null) return Result.Failure<AdminSideUpdatePaymentDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result> AdminSideUpdatePaymentData(AdminSideUpdatePaymentDataViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var data = await _dbSet.FindAsync(1);
            if (data is null) return Result.Failure(Messages.OperationFailedError);

            data.Tax = model.Tax;
            data.PaymentGatewayPin = model.PaymentGatewayPin;

            _dbSet.Update(data);
            await _context.SaveChangesAsync();

            PaymentDataUpdatedEvent PaymentDataUpdatedEvent = new PaymentDataUpdatedEvent(
                data.Id,
                data.Tax.Value,
                data.PaymentGatewayPin);

            await _mediator.PublishEvent(PaymentDataUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<int>> GetTaxRate()
        {
            var taxRate = await _dbSet.Where(n => n.Id == 1).Select(n => n.Tax).FirstOrDefaultAsync();
            if (taxRate < 1) return Result.Failure<int>(Messages.OperationFailedError);

            return taxRate;
        }

        public async Task<Result<string>> GetPin()
        {
            var pin = await _dbSet.Where(n => n.Id == 1).Select(n => n.PaymentGatewayPin).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(pin)) return Result.Failure<string>(Messages.OperationFailedError);

            return pin;
        }
    }
}
