using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideAddCouponToCartViewModel
    {
        [Display(Name = "کد تخفیف", Prompt = "کد تخفیف")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Code { get; set; }
    }
}
