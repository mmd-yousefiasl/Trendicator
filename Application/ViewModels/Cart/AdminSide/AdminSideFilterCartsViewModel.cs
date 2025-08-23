using Domain.Enums.Shopping;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Cart.AdminSide
{
    public class AdminSideFilterCartsViewModel : BasePaging<AdminSideCartViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت")]
        public CartStatusEnum? Status { get; set; }
    }
}
