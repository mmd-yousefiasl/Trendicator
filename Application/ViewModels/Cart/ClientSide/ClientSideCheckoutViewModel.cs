using Application.ValidationAttributes;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Cart.ClientSide
{
    public class ClientSideCheckoutViewModel
    {
        public bool UseWallet { get; set; }
        public long Balance { get; set; }

        [Display(Name = "سیاست حفظ حریم خصوصی و شرایط و ظوابط")]
        [MustBeTrue(ErrorMessage = Messages.RequiredError)]
        public bool PrivacyPolicy { get; set; }
    }
}
