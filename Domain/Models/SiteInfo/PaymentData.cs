namespace Domain.Models.SiteInfo
{
    public class PaymentData : BaseEntity<int>
    {
        public int? Tax { get; set; }
        public string PaymentGatewayPin { get; set; }
    }
}
