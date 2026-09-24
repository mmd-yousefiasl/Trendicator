using Domain.Models;

namespace Application.Dtos.ProductOption.AdminSide
{
    public class AdminSideFilterProductOptionsViewModel : BasePaging<AdminSideProductOptionViewModel>
    {
        public int ProductId { get; set; }
    }
}
