using Domain.Models;

namespace Application.ViewModels.ProductOption.AdminSide
{
    public class AdminSideFilterProductOptionsViewModel : BasePaging<AdminSideProductOptionViewModel>
    {
        public int ProductId { get; set; }
    }
}
