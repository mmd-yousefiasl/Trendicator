using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductCategory.AdminSide
{
    public class AdminSideUpdateProductCategoryViewModel : CommonViewModel
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "عنوان در مرورگر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Slug { get; set; }

        [Display(Name = "دسته بندی والد")]
        public int? ParentCategoryId { get; set; }
    }
}
