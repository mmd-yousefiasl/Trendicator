using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.PaymentData
{
    public class AdminSideUpdatePaymentDataViewModel
    {
        [Display(Name = "پین درگاه پرداخت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string PaymentGatewayPin { get; set; }
    }
}
