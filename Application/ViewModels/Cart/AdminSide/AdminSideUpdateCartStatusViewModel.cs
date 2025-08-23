using Domain.Enums.Shopping;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Cart.AdminSide
{
    public class AdminSideUpdateCartStatusViewModel : CommonViewModel
    {
        [Display(Name = "وضعیت سفارش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public CartStatusEnum Status { get; set; }
    }
}
