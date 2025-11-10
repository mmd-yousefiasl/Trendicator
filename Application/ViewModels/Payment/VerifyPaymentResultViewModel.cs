using Domain.Enums.Payment;

namespace Application.ViewModels.Payment
{
    public class VerifyPaymentResultViewModel
    {
        public DateTime paidAt { get; set; }
        public string? cardNumber { get; set; }
        public VerifyPaymentResultStatusEnum status { get; set; }
        public long amount { get; set; }
        public string? refNumber { get; set; }
        public string? description { get; set; }
        public string? orderId { get; set; }
        public int result { get; set; }
        public string? message { get; set; }
    }
}
