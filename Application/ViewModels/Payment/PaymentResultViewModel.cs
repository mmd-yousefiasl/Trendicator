using Domain.Enums.Payment;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Payment
{
    public class PaymentResultViewModel
    {
        [Display(Name = "کد تراکنش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string transid { get; set; }

        [Display(Name = "وضعیت تراکنش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public PaymentResultStatusEnum status { get; set; }
    }
}
