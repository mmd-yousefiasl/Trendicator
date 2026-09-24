using Application.Dtos;
using Domain.Enums.Shopping;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cart.AdminSide
{
    public class AdminSideUpdateCartStatusViewModel : CommonDto
    {
        [Display(Name = "وضعیت سفارش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public CartStatusEnum Status { get; set; }
    }
}
