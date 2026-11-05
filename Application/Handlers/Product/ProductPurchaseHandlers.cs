using Application.Services.Interfaces;
using Domain.Enums.Shopping;
using Domain.Events.Shopping;
using MediatR;

namespace Application.Handlers.Product
{
    public class ProductPurchaseHandlers : INotificationHandler<CartPaidEvent>
    {
        private readonly IProductService _productService;
        public ProductPurchaseHandlers(IProductService productService)
        {
            _productService = productService;
        }

        public async Task Handle(CartPaidEvent notification, CancellationToken cancellationToken)
        {
            if (notification.ProductIds is not null && notification.ProductIds.Count > 0 && notification.Status == CartStatusEnum.Paid)
                await _productService.ClientSideUpdateProductsPurchaseCount(notification.ProductIds);
        }
    }
}
