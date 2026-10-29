using Domain.Models;

namespace Application.Dtos.ProductDetails.AdminSide
{
    public class AdminSideFilterProductDetailsDto : BasePaging<AdminSideProductDetailsDto>
    {
        public int ProductId { get; set; }
    }
}
