using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Enums.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VerifyPaymentResultStatusEnum
    {
        [Display(Name = "خطا")]
        Error = 0,
        [Display(Name = "موفق")]
        Success = 1,
        [Display(Name = "تراکنش قبلا وریفای و پرداخت شده است")]
        AlreadyVerified = 2
    }
}
