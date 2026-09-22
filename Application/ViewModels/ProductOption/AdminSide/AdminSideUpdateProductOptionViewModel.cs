using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductOption.AdminSide
{
    public class AdminSideUpdateProductOptionViewModel : CommonDto
    {
        [Display(Name = "محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int ProductId { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public long Price { get; set; }

        [Display(Name = "قیمت سفارش")]
        public long? OrderPrice { get; set; }
    }
}
