using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideUpdateTradingViewAccountNameViewModel
    {
        public bool IsAllowed { get; set; }

        [Display(Name = "نام اکانت TradingView (تریدینگ ویو) شما", Prompt = "نام اکانت تریدینگ ویو ...")]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string TradingViewAccountName { get; set; }
    }
}
