using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Faq.AdminSide
{
    public class AdminSideUpdateFaqViewModel : CommonViewModel
    {
        [Display(Name = "سوال")]
        [MaxLength(300)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Question { get; set; }

        [Display(Name = "پاسخ")]
        [MaxLength(800)]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Answer { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }
    }
}
