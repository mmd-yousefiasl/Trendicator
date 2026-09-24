using Domain.Models;

namespace Application.Dtos.ProductFaq.AdminSide
{
    public class AdminSideFilterProductFaqsViewModel : BasePaging<AdminSideProductFaqViewModel>
    {
        public int ProductId { get; set; }
    }
}
