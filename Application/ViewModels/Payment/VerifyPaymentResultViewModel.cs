using Domain.Enums.Payment;

namespace Application.ViewModels.Payment
{
    public class VerifyPaymentResultViewModel
    {
        public string Status { get; set; }
        public VerifyPaymentResultStatusEnum Code { get; set; }
    }
}
