using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientApplyCouponDto
    {
        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Code { get; set; }
    }
}
