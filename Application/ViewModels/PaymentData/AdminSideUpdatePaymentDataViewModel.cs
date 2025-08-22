using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.PaymentData
{
    public class AdminSideUpdatePaymentDataViewModel
    {
        [Display(Name = "درصد مالیات")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(0, 100, ErrorMessage = Messages.RangeError)]
        public int Tax { get; set; }

        [Display(Name = "پین درگاه پرداخت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string PaymentGatewayPin { get; set; }
    }
}
