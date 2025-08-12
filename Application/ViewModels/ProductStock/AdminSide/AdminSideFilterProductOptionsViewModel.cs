using Domain.Models;

namespace Application.ViewModels.ProductStock.AdminSide
{
    public class AdminSideFilterProductOptionsViewModel : BasePaging<AdminSideProductOptionViewModel>
    {
        public int ProductId { get; set; }
    }
}
