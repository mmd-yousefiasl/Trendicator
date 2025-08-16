using Domain.Enums.Products;
using MediatR;

namespace Domain.Events.Products.ProductReview
{
    public record ProductReviewCreatedEvent(
    string UserName,
    string Review,
    ProductReviewStarsEnum Stars,
    bool IsConfirmed,
    string PhoneNumber,
    int? UserId,
    string UserIP,
    int ProductId
    ) : INotification;
}
