using System.ComponentModel.DataAnnotations;
using Application.ValidationAttributes;
using Domain.Shared;

namespace Application.Dtos.Cart.ClientSide
{
    public class ClientSideCheckoutViewModel
    {
        public bool UseWallet { get; set; }
        public decimal Balance { get; set; }

        [Display(Name = "سیاست حفظ حریم خصوصی و شرایط و ظوابط")]
        [MustBeTrue(ErrorMessage = Messages.RequiredError)]
        public bool PrivacyPolicy { get; set; }
    }
}
