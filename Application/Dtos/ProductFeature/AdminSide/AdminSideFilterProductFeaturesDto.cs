using Domain.Models;

namespace Application.Dtos.ProductFeature.AdminSide
{
    public class AdminSideFilterProductFeaturesDto : BasePaging<AdminSideProductFeatureDto>
    {
        public int ProductId { get; set; }
    }
}
