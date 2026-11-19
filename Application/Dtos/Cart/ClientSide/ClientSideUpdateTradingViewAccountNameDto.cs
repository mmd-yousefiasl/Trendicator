using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideUpdateTradingViewAccountNameDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }

        [Display(Name = "نام اکانت تریدینگ ویو", Prompt = "User_M...")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.RequiredError)]
        public string? TradingViewAccountName { get; set; }
    }
}
