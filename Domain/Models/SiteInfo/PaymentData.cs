namespace Domain.Models.SiteInfo
{
    public class PaymentData : BaseEntity<int>
    {
        public string PaymentGatewayPin { get; set; }
    }
}
