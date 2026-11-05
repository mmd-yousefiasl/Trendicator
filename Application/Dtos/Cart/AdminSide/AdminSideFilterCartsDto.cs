using Domain.Enums.Shopping;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cart.AdminSide
{
    public class AdminSideFilterCartsDto : BasePaging<AdminSideCartDto>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت")]
        public CartStatusEnum? Status { get; set; }
    }
}
