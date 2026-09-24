namespace Application.Dtos.Payment
{
    public class PaymentRequestViewModel
    {
        public decimal Amount { get; set; }
        public string CallBackUrl { get; set; }
    }
}
