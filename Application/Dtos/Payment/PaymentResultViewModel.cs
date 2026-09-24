using Domain.Enums.Payment;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Payment
{
    public class PaymentResultViewModel
    {
        [Display(Name = "وضعیت تراکنش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int success { get; set; }

        [Display(Name = "کد تراکنش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public long trackId { get; set; }

        public PaymentResultStatusEnum status { get; set; }
    }
}
