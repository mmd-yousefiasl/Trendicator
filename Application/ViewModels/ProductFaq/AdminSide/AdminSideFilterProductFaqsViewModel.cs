using Domain.Models;

namespace Application.ViewModels.ProductFaq.AdminSide
{
    public class AdminSideFilterProductFaqsViewModel : BasePaging<AdminSideProductFaqViewModel>
    {
        public int ProductId { get; set; }
    }
}
